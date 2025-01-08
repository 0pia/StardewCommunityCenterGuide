using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using System.Runtime.Remoting.Messaging;

namespace project3 {
    enum ItemType {
        Fish,
        Plant,
        Other,
    }

    // probably not necessary, but makes it more readable i think
    enum FieldType {
        Int,
        String,
        Bool,
    }

    // parameter for query, these are basically the same
    enum OperationType {
        Query,
        Delete,
        Modify,
    }

    // should store size for better input handling
    struct Field {
        public string name;
        public FieldType type;
        public int size;

        public Field(string name, FieldType type, int size) {
            this.name = name;
            this.type = type;
            this.size = size;
        }
    }

    class Program {
        // store the name of the tables for ease of modification
        static readonly string[] tableNamesArray = { "bundles", "fish", "plant", "items" };
        static readonly List<string> tableNames = new List<string>(tableNamesArray);

        static readonly Dictionary<ItemType, string> itemTypeNames = new Dictionary<ItemType, string> {
            { ItemType.Fish, "hal" },
            { ItemType.Plant, "növény" },
            { ItemType.Other, "egyéb" },
        };

        // connect to database server, create database if doesn't exist yet
        // might be better to store connection handle in global variable, but it's more flexible this way
        static MySqlConnection ConnectToDatabase() {
            string connectionString = "server=localhost;uid=root;password='';";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string createDbString = "CREATE DATABASE IF NOT EXISTS stardew_valley CHARACTER SET utf8 COLLATE utf8_hungarian_ci; use stardew_valley;";
            MySqlCommand createDb = new MySqlCommand(createDbString, connection);
            createDb.ExecuteNonQuery();
            createDb.Dispose();

            // fill DB if empty
            string fillDbString = "SHOW TABLES;";
            MySqlCommand fillDb = new MySqlCommand(fillDbString, connection);
            MySqlDataReader reader = fillDb.ExecuteReader();

            int numberOfFields = 0;
            while(reader.Read()) {
                numberOfFields++;
            }

            reader.Close();
            fillDb.Dispose();

            if(numberOfFields == 0) {
                StreamReader initialDbReader = new StreamReader("./default.sql");
                MySqlCommand initDb = new MySqlCommand(initialDbReader.ReadToEnd(), connection);
                initDb.ExecuteNonQuery();

                initDb.Dispose();
                initialDbReader.Close();
            }

            return connection;
        }

        // read name of save from user, validate that it's alphanumeric
        static string GetSaveName() {
            Console.CursorVisible = true;
            Console.Write("Mentés neve: ");
            string savename = Console.ReadLine().Trim();
            Regex rg = new Regex(@"^[a-zA-Z0-9_]+$");
            if(!rg.IsMatch(savename) || savename.Length > 100) {
                Console.WriteLine("Hibás név");
                savename = GetSaveName();
            }
            Console.CursorVisible = false;
            return savename;
        }

        // fetch all data, write each table's data in its own .csv file
        static void WriteDatabaseToFile(MySqlConnection connection) {
            string savename = GetSaveName();

            try {
                Directory.CreateDirectory(savename);
            } catch(Exception e) {
                // if there was an error in creating directory, save to already existing 'failsafe' directory
                savename = "failsafe";
                Console.SetCursorPosition(0, Console.WindowHeight - 2);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"ERROR: Failed to create directory. Using 'failsafe' as fallback. Error message:\n{e.Message}");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadKey(true);
            }

            for(int i = 0; i < tableNames.Count; i++) {
                StreamWriter writer = new StreamWriter(Path.Combine(savename, $"{tableNames[i]}.csv"));

                MySqlCommand queryAll = new MySqlCommand($"SELECT * FROM {tableNames[i]};", connection);
                MySqlDataReader reader = queryAll.ExecuteReader();

                while(reader.Read()) {
                    for(int field = 0; field < reader.FieldCount; field++) {
                        writer.Write($"{reader[field]}");
                        if(field != reader.FieldCount - 1) {
                            writer.Write(";");
                        }
                    }
                    writer.WriteLine();
                }

                queryAll.Dispose();
                reader.Close();
                writer.Close();
            }
        }

        // read data from the saved .csv files, upload it to the DB
        static void ReadDatabaseFromFile(MySqlConnection connection) {
            string savename = GetSaveName();

            for(int i = 0; i < tableNames.Count; i++) {
                StreamReader reader;
                try {
                    reader = new StreamReader(Path.Combine(savename, $"{tableNames[i]}.csv"));
                } catch {
                    Console.WriteLine("Nem létezik mentés ilyen névvel.");
                    return;
                }

                MySqlCommand deleteAll = new MySqlCommand($"DELETE FROM {tableNames[i]};", connection);
                deleteAll.ExecuteNonQuery();
                deleteAll.Dispose();

                List<Field> fields = GetFields(tableNames[i], connection, true);

                string insert = $"INSERT INTO {tableNames[i]} (";
                foreach(Field field in fields) {
                    insert += $"{field.name}, ";
                }
                insert = insert.Remove(insert.Length - 2, 2) + ") VALUES\n";

                while(!reader.EndOfStream) {
                    string[] line = reader.ReadLine().Trim().Split(';');
                    insert += "(";
                    for(int s = 0; s < line.Length; s++) {
                        string wrap = fields[s].type == FieldType.String ? "\"" : "";
                        if(line[s] == "") {
                            line[s] = "NULL";
                            wrap = "";
                        }
                        insert += $"{wrap}{line[s]}{wrap}";
                        if(s < line.Length - 1) {
                            insert += ", ";
                        } else {
                            insert += "),\n";
                        }
                    }
                }
                insert = insert.Remove(insert.Length - 2, 2) + ";";
                MySqlCommand insertCommand = new MySqlCommand(insert, connection);
                insertCommand.ExecuteNonQuery();
                insertCommand.Dispose();

                reader.Close();
            }
        }

        // generic menu function that returns the index of the selected item
        static int GenericSelectionMenu(string title, List<string> items) {
            bool quit = false;
            int activeMenuItem = 0;

            ConsoleKeyInfo input;
            do {
                // clear console before writing
                // notes: Console.Clear() is slow, causes flickering
                // workaround: buffer and window are the same size, just scroll buffer
                for(int i = 0; i < Console.WindowHeight; i++) {
                    Console.WriteLine();
                }
                Console.SetCursorPosition(0, 0);

                Console.WriteLine($"== {title} ==");
                for(int i = 0; i < items.Count; i++) {
                    char menuItemSymbol = activeMenuItem == i ? '*' : ' ';
                    Console.WriteLine($"[{menuItemSymbol}] {items[i]}");
                }

                input = Console.ReadKey(true);

                switch(input.Key) {
                    case ConsoleKey.DownArrow:
                        if(activeMenuItem < items.Count - 1) {
                            activeMenuItem++;
                        } else {
                            activeMenuItem = 0;
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        if(activeMenuItem > 0) {
                            activeMenuItem--;
                        } else {
                            activeMenuItem = items.Count - 1;
                        }
                        break;
                    case ConsoleKey.Enter:
                        quit = true;
                        break;
                }
            } while(!quit);
            return activeMenuItem;
        }

        // the main menu of the program
        static void Menu(MySqlConnection connection) {
            bool quit = false;
            string title = "Főmenü";
            string[] items = {
                "Új tárgy bevitele",
                "Szűrés",
                "Tulajdonság frissítése",
                "Tárgy törlése",
                "Teljes állomány megtekintése",
                "Statisztika",
                "Ajánló funkciók",
                "Mentés fileba",
                "Beolvasás fileból",
                "Kilépés"
            };

            while(!quit) {
                switch(GenericSelectionMenu(title, items.ToList())) {
                    case 0:
                        NewItem(connection);
                        break;

                    case 1:
                        Query(connection, OperationType.Query);
                        break;

                    case 2:
                        Query(connection, OperationType.Modify);
                        break;

                    case 3:
                        Query(connection, OperationType.Delete);
                        break;

                    case 4:
                        ShowAll(connection);
                        break;

                    case 5:
                        Statistics(connection);
                        break;

                    case 6:
                        Suggested(connection);
                        break;

                    case 7:
                        WriteDatabaseToFile(connection);
                        break;

                    case 8:
                        ReadDatabaseFromFile(connection);
                        break;

                    case 9:
                        quit = true;
                        break;
                }
            }
        }

        // query field information from DB
        static List<Field> GetFields(string table, MySqlConnection connection, bool includeAll) {
            string query = $"SHOW COLUMNS FROM {table};";
            MySqlCommand getFieldNames = new MySqlCommand(query, connection);
            MySqlDataReader reader = getFieldNames.ExecuteReader();

            List<Field> fields = new List<Field>();
            // using a HashSet is way overkill but it's easier and performance/memory isn't a concern yet
            HashSet<string> unrecognisedFields = new HashSet<string>();

            while(reader.Read()) {
                string fieldName = reader[0].ToString();
                FieldType type;

                if(reader[1].ToString().ToLower().Contains("varchar")) {
                    type = FieldType.String;
                } else if(reader[1].ToString().ToLower().Contains("int")) {
                    type = FieldType.Int;
                } else if(reader[1].ToString().ToLower().Contains("bit")) {
                    type = FieldType.Bool;
                } else {
                    unrecognisedFields.Add(reader[1].ToString());
                    type = FieldType.String;
                }
                Regex sizeRegex = new Regex(@"[0-9]+");
                int size = int.Parse(sizeRegex.Match(reader[1].ToString()).Value);

                if(table == "bundles" && !includeAll) {
                    if(fieldName != "BundleID") fields.Add(new Field(fieldName, type, size));
                } else {
                    fields.Add(new Field(fieldName, type, size));
                }
            }

            getFieldNames.Dispose();
            reader.Close();

            // display warning if there are fields with unrecognised type. shouldn't happen.
            if(unrecognisedFields.Count > 0) {
                string warningMessage = "WARNING: some field types were not recognized: ";
                foreach(string item in unrecognisedFields) {
                    warningMessage += $"{item}\t";
                }
                warningMessage = warningMessage.Trim();
                warningMessage += "\nusing string as fallback. press any key to continue.";
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(0, Console.WindowHeight - 2);
                Console.Write(warningMessage);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadKey(true);
            }

            return fields;
        }

        // add a new item to the database
        static void NewItem(MySqlConnection connection) {
            string title = "Új tárgy bevitele";
            List<string> items = new List<string>();

            // create list from ItemType
            Array values = Enum.GetValues(typeof(ItemType));
            foreach(ItemType type in values) {
                items.Add(itemTypeNames[type]);
            }
            items.Add("Vissza");

            int selected = GenericSelectionMenu(title, items.ToList());
            if(selected == items.Count - 1) return;
            string tableName = tableNames[selected + 1];

            List<Field> fields = GetFields(tableName, connection, false);
            List<string> newValues = new List<string>();

            foreach(Field field in fields) {
                string prompt = field.name == "BundleID" ? "bundle" : field.name;
                Console.Write($"{prompt}: ");
                Console.CursorVisible = true;
                string criteria;
                if(field.name == "BundleID") {
                    criteria = GetBundleID(connection, Console.ReadLine());
                    if(criteria == "None") return;
                } else if(field.type == FieldType.Bool) {
                    string[] options = { "igen", "nem" };
                    criteria = (1 - GenericSelectionMenu(field.name, options.ToList())).ToString();
                } else if(field.type == FieldType.Int) {
                    Regex isNumber = new Regex(@"[0-9]+");
                    criteria = Console.ReadLine().Trim();
                    if(!isNumber.IsMatch(criteria)) {
                        Console.WriteLine("Bemenet nem értelmezhető számként. Alapérték (NULL) kerül beírásra.");
                        criteria = "NULL";
                    }
                } else {
                    criteria = Console.ReadLine().Trim();
                }
                newValues.Add(criteria);
                Console.CursorVisible = false;
            }

            string insert = $"INSERT INTO {tableName} (";
            foreach(Field field in fields) {
                insert += $"{field.name}, ";
            }
            insert = insert.Remove(insert.Length - 2, 2) + ") VALUES\n";

            insert += "(";
            for(int s = 0; s < newValues.Count; s++) {
                // turns out if mysql sees "0" at a bit(1) type field it resolves it to 1
                string wrap = fields[s].type == FieldType.String ? "\"" : "";
                insert += $"{wrap}{newValues[s]}{wrap}";
                if(s < newValues.Count - 1) {
                    insert += ", ";
                } else {
                    insert += "),\n";
                }
            }
            insert = insert.Remove(insert.Length - 2, 2) + ";";
            MySqlCommand insertCommand = new MySqlCommand(insert, connection);
            if(insertCommand.ExecuteNonQuery() == 1) {
                Console.WriteLine("Elem sikeresen hozzáadva.");
            } else {
                Console.WriteLine("Elem hozzáadása nem sikertelen.");
            }
            insertCommand.Dispose();
            Console.ReadKey();
        }

        static string GetBundleID(MySqlConnection connection, string bundleName) {
            string query = $"SELECT BundleID FROM bundles WHERE Name LIKE \"{bundleName}\"";
            MySqlCommand getBundleID = new MySqlCommand(query, connection);
            MySqlDataReader reader = getBundleID.ExecuteReader();
            reader.Read();
            string bundleID;
            try {
                bundleID = reader[0].ToString();
            } catch {
                bundleID = "None";
                Console.WriteLine("Nincs ilyen bundle. Nyomjon le egy billentyűt a folytatáshoz...");
                Console.ReadKey(true);
            }
            reader.Close();
            getBundleID.Dispose();
            return bundleID;
        }

        // query DB with user-defined criteria or delete records
        static void Query(MySqlConnection connection, OperationType operationType) {
            string title = "";
            switch(operationType) {
                case OperationType.Query:
                    title = "Szűrés";
                    break;
                case OperationType.Delete:
                    title = "Törlés";
                    break;
                case OperationType.Modify:
                    title = "Mi alapján frissít?";
                    break;
            }
            List<string> tableMenu = new List<string>(tableNames);
            if(tableMenu.Last() != "Vissza") tableMenu.Add("Vissza");

            int selectedTable = GenericSelectionMenu("Tábla", tableMenu);
            if(selectedTable == tableMenu.Count - 1) return;
            string tableName = tableNames[selectedTable];
            List<Field> fields = GetFields(tableName, connection, false);
            List<string> items = new List<string>();

            // create checkbox from field names to use as criteria
            foreach(Field field in fields) {
                if(tableName != "bundles") {
                    items.Add(field.name == "BundleID" ? "bundle" : field.name);
                } else {
                    if(field.name != "BundleID") items.Add(field.name);
                }
            }
            items.Add("Vissza");

            //int selected = GenericSelectionMenu(title, items.ToList());
            //if(selected == items.Count - 1) return;

            bool[] selectedItems = new bool[items.Count];
            int activeMenuItem = 0;
            int numberOfSelected = 0;

            // this should be a generic menu function, reused
            // but it's a checkbox, not technically a menu
            ConsoleKeyInfo input;
            do {
                for(int i = 0; i < Console.WindowHeight; i++) {
                    Console.WriteLine();
                }
                Console.SetCursorPosition(0, 0);

                Console.WriteLine($"== {title} ==");
                for(int i = 0; i < items.Count; i++) {
                    char menuItemSymbol = activeMenuItem == i ? '*' : selectedItems[i] ? '+' : ' ';
                    if(selectedItems[i]) {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.WriteLine($"[{menuItemSymbol}] {items[i]}");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                input = Console.ReadKey(true);

                switch(input.Key) {
                    case ConsoleKey.DownArrow:
                        if(activeMenuItem < items.Count - 1) {
                            activeMenuItem++;
                        } else {
                            activeMenuItem = 0;
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        if(activeMenuItem > 0) {
                            activeMenuItem--;
                        } else {
                            activeMenuItem = items.Count - 1;
                        }
                        break;

                    case ConsoleKey.Spacebar:
                        if(activeMenuItem != items.Count - 1) selectedItems[activeMenuItem] = !selectedItems[activeMenuItem];
                        break;
                }
            } while(input.Key != ConsoleKey.Enter);

            foreach(bool isSelected in selectedItems) {
                if(isSelected) numberOfSelected++;
            }

            // process selected items

            if(activeMenuItem == items.Count - 1) return;

            string operation = "";
            switch(operationType) {
                case OperationType.Query:
                    operation = tableName == "bundles" ? "SELECT bundles.Name, bundles.Reward, bundles.Needs, bundles.Max FROM" : "SELECT * FROM";
                    break;
                case OperationType.Delete:
                    operation = "DELETE FROM";
                    break;
                case OperationType.Modify:
                    operation = "UPDATE";
                    break;
            }

            string queryString = $"{operation} {tableName} ";

            int counter = 0;
            string where = "WHERE ";
            for(int i = 0; i < items.Count; i++) {
                if(selectedItems[i]) {
                    counter++;
                    string comperator = fields[i].type == FieldType.String || fields[i].name == "BundleID" ? "LIKE" : "=";
                    string prompt = $"{items[i]}: ";
                    Console.Write(prompt);
                    Console.CursorVisible = true;
                    string criteria;
                    if(fields[i].name == "BundleID") {
                        criteria = GetBundleID(connection, Console.ReadLine());
                        if(criteria == "None") return;
                    } else if(fields[i].type == FieldType.Bool) {
                        string[] options = { "igen", "nem" };
                        criteria = (1 - GenericSelectionMenu(fields[i].name, options.ToList())).ToString();
                    } else if(fields[i].type == FieldType.Int) {
                        Regex isNumber = new Regex(@"[0-9]+");
                        criteria = Console.ReadLine().Trim();
                        if(!isNumber.IsMatch(criteria)) {
                            Console.WriteLine("Bemenet nem értelmezhető számként és kihagyásra kerül.");
                            criteria = "%";
                        }
                    } else {
                        criteria = Console.ReadLine().Trim();
                    }
                    string wrap = fields[i].type == FieldType.String ? "\"" : "";
                    where += $"{items[i]} {comperator} {wrap}{criteria}{wrap}";
                    if(counter == numberOfSelected) {
                        where += ";";
                    } else {
                        where += " AND ";
                    }
                    Console.CursorVisible = false;
                }
            }

            // construct the SET part of the UPDATE query
            if(operationType == OperationType.Modify) {
                title = "Mit frissít?";
                selectedItems = new bool[items.Count];
                activeMenuItem = 0;
                numberOfSelected = 0;

                // this should be a generic menu function, reused
                // but it's a checkbox, not technically a menu
                do {
                    for(int i = 0; i < Console.WindowHeight; i++) {
                        Console.WriteLine();
                    }
                    Console.SetCursorPosition(0, 0);

                    Console.WriteLine($"== {title} ==");
                    for(int i = 0; i < items.Count; i++) {
                        char menuItemSymbol = activeMenuItem == i ? '*' : selectedItems[i] ? '+' : ' ';
                        if(selectedItems[i]) {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                        Console.WriteLine($"[{menuItemSymbol}] {items[i]}");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    input = Console.ReadKey(true);

                    switch(input.Key) {
                        case ConsoleKey.DownArrow:
                            if(activeMenuItem < items.Count - 1) {
                                activeMenuItem++;
                            } else {
                                activeMenuItem = 0;
                            }
                            break;

                        case ConsoleKey.UpArrow:
                            if(activeMenuItem > 0) {
                                activeMenuItem--;
                            } else {
                                activeMenuItem = items.Count - 1;
                            }
                            break;

                        case ConsoleKey.Spacebar:
                            selectedItems[activeMenuItem] = !selectedItems[activeMenuItem];
                            break;
                    }
                } while(input.Key != ConsoleKey.Enter);

                foreach(bool isSelected in selectedItems) {
                    if(isSelected) numberOfSelected++;
                }

                int setCounter = 0;
                string set = "SET ";
                for(int i = 0; i < items.Count; i++) {
                    if(selectedItems[i]) {
                        setCounter++;
                        string prompt = $"{items[i]}: ";
                        Console.Write(prompt);
                        Console.CursorVisible = true;
                        string criteria;
                        if(fields[i].name == "BundleID") {
                            criteria = GetBundleID(connection, Console.ReadLine());
                            if(criteria == "None") return;
                        } else if(fields[i].type == FieldType.Bool) {
                            string[] options = { "igen", "nem" };
                            criteria = (1 - GenericSelectionMenu(fields[i].name, options.ToList())).ToString();
                        } else if(fields[i].type == FieldType.Int) {
                            Regex isNumber = new Regex(@"[0-9]+");
                            criteria = Console.ReadLine().Trim();
                            if(!isNumber.IsMatch(criteria)) {
                                Console.WriteLine("Bemenet nem értelmezhető számként és kihagyásra kerül.");
                                criteria = "%";
                            }
                        } else {
                            criteria = Console.ReadLine().Trim();
                        }
                        string wrap = fields[i].type == FieldType.String ? "\"" : "";
                        set += $"{items[i]} = {wrap}{criteria}{wrap}";
                        if(setCounter == numberOfSelected) {
                            set += " ";
                        } else {
                            set += ", ";
                        }
                        Console.CursorVisible = false;
                    }
                }
                queryString += set;
                if(setCounter == 0) {
                    Console.WriteLine("Nem frissült egy adat sem mert nem adott meg új értékeket.");
                    return;
                }
            }

            queryString += where;

            if(counter == 0) queryString += "1;";

            MySqlCommand select = new MySqlCommand(queryString, connection);
            if(operationType == OperationType.Query) {
                MySqlDataReader selectReader = select.ExecuteReader();
                List<(string, int)> output = new List<(string, int)>();
                for(int field = 0; field < fields.Count; field++) {
                    output.Add((fields[field].name, fields[field].size));
                }
                output.Add(("\n\n", 0));

                while(selectReader.Read()) {
                    for(int field = 0; field < selectReader.FieldCount; field++) {
                        Console.WriteLine(selectReader[field].ToString());
                        Console.WriteLine(fields[field].size.ToString());
                        output.Add((selectReader[field].ToString(), fields[field].size));
                    }
                    output.Add(("\n", 0));
                }
                DisplayValues(output, false);

                select.Dispose();
                selectReader.Close();
            } else {
                int rows = select.ExecuteNonQuery();
                select.Dispose();
                string operationName = operationType == OperationType.Delete ? "törölve" : "módosítva";
                Console.WriteLine($"{rows} elem {operationName}. Nyomjon le egy billentyűt a folytatáshoz...");
                Console.ReadKey(true);
            }
        }

        // query every record from the DB
        static void ShowAll(MySqlConnection connection) {
            Dictionary<string, string> queries = new Dictionary<string, string> {
                { "bundles", "SELECT bundles.Name, bundles.Reward, bundles.Needs, bundles.Max FROM bundles;" },
                {
                    "fish",
                    "SELECT fish.name, fish.spring, fish.summer, fish.fall, fish.winter, fish.place, fish.weather, fish.time, fish.has, fish.needs, bundles.Name " +
                "FROM fish INNER JOIN bundles ON fish.BundleID = bundles.BundleID;"
                },
                {
                    "plant",
                    "SELECT plant.name, plant.spring, plant.summer, plant.fall, plant.winter, plant.category, plant.toGrow, plant.toFind, plant.has, plant.needs, bundles.Name " +
                "FROM plant INNER JOIN bundles ON plant.BundleID = bundles.BundleID;"
                },
                {
                    "items",
                    "SELECT items.name, items.category, items.preItem, items.getFrom, items.has, items.needs, bundles.Name " +
                "FROM items INNER JOIN bundles ON items.BundleID = bundles.BundleID;"
                }
            };
            Console.BufferHeight = 3000;
            Console.BufferWidth = 300;
            Console.WindowHeight++;
            Console.Clear();

            foreach(string tableName in tableNames) {
                List<Field> fields = GetFields(tableName, connection, false);

                MySqlCommand select = new MySqlCommand(queries[tableName], connection);
                MySqlDataReader selectReader = select.ExecuteReader();
                List<(string, int)> output = new List<(string, int)>();
                for(int field = 0; field < fields.Count; field++) {
                    output.Add((fields[field].name, fields[field].size));
                }
                output.Add(("\n\n", 0));

                while(selectReader.Read()) {
                    for(int field = 0; field < selectReader.FieldCount; field++) {
                        output.Add((selectReader[field].ToString(), fields[field].size));
                    }
                    output.Add(("\n", 0));
                }
                DisplayValues(output, true);
                Console.WriteLine();

                select.Dispose();
                selectReader.Close();
            }

            Console.Write("\nNyomjon le egy billentyűt a folytatáshoz...");
            Console.ReadKey(true);
            Console.Clear();
            Console.WindowHeight--;
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
        }

        // get some statistics, print them to screen
        static void Statistics(MySqlConnection connection) {
            string[] seasons = { "spring", "summer", "fall", "winter" };
            string[] seasonsText = { "tavasz", "nyár", "ősz", "tél" };
            int[] fishPerSeason = new int[seasons.Length];
            for(int i = 0; i < seasons.Length; i++) {
                MySqlCommand queryFish = new MySqlCommand($"SELECT COUNT({seasons[i]}) FROM fish WHERE {seasons[i]} = 1;", connection);
                MySqlDataReader fishReader = queryFish.ExecuteReader();

                fishReader.Read();
                try {
                    fishPerSeason[i] = int.Parse(fishReader[0].ToString());
                } catch {
                    fishPerSeason[i] = 0;
                }

                fishReader.Close();
                queryFish.Dispose();
            }

            int[] plantPerSeason = new int[seasons.Length];
            for(int i = 0; i < seasons.Length; i++) {
                MySqlCommand queryPlant = new MySqlCommand($"SELECT COUNT({seasons[i]}) FROM plant WHERE {seasons[i]} = 1;", connection);
                MySqlDataReader plantReader = queryPlant.ExecuteReader();

                plantReader.Read();
                try {
                    plantPerSeason[i] = int.Parse(plantReader[0].ToString());
                } catch {
                    plantPerSeason[i] = 0;
                }

                plantReader.Close();
                queryPlant.Dispose();
            }

            Console.Clear();
            Console.WriteLine("== Statisztika ==\n");
            Console.WriteLine($"Ebben a hónapban van a legtöbb hal: {seasonsText[fishPerSeason.ToList().IndexOf(fishPerSeason.Max())]}");
            Console.WriteLine($"Ebben a hónapban van a legtöbb növény: {seasonsText[plantPerSeason.ToList().IndexOf(plantPerSeason.Max())]}");
            Console.Write("\nNyomjon le egy billentyűt a folytatáshoz...");
            Console.ReadKey(true);
        }

        // some useful queries in quick-access
        static void Suggested(MySqlConnection connection) {
            string[] items = {
                "Több helyről kifogható halak",
                "Több helyről beszerezhető tárgyak",
                "Halak amiből még nincs elég",
                "Tárgyak amiből még nincs elég",
                "Növények amiből még nincs elég",
                "Vissza"
            };
            switch(GenericSelectionMenu("Ajánló funkciók", items.ToList())) {
                case 0:
                    {
                        List<Field> fields = GetFields("fish", connection, false);
                        string query = "SELECT * FROM fish WHERE place LIKE \"%/%\";";

                        MySqlCommand select = new MySqlCommand(query, connection);
                        MySqlDataReader selectReader = select.ExecuteReader();
                        List<(string, int)> output = new List<(string, int)>();
                        for(int field = 0; field < fields.Count; field++) {
                            output.Add((fields[field].name, fields[field].size));
                        }
                        output.Add(("\n\n", 0));

                        while(selectReader.Read()) {
                            for(int field = 0; field < selectReader.FieldCount; field++) {
                                output.Add((selectReader[field].ToString(), fields[field].size));
                            }
                            output.Add(("\n", 0));
                        }
                        DisplayValues(output, false);

                        select.Dispose();
                        selectReader.Close();
                    }
                    break;
                case 1:
                    {
                        List<Field> fields = GetFields("items", connection, false);
                        string query = "SELECT * FROM items WHERE getFrom LIKE \"%/%\";";

                        MySqlCommand select = new MySqlCommand(query, connection);
                        MySqlDataReader selectReader = select.ExecuteReader();
                        List<(string, int)> output = new List<(string, int)>();
                        for(int field = 0; field < fields.Count; field++) {
                            output.Add((fields[field].name, fields[field].size));
                        }
                        output.Add(("\n\n", 0));

                        while(selectReader.Read()) {
                            for(int field = 0; field < selectReader.FieldCount; field++) {
                                output.Add((selectReader[field].ToString(), fields[field].size));
                            }
                            output.Add(("\n", 0));
                        }
                        DisplayValues(output, false);

                        select.Dispose();
                        selectReader.Close();
                    }
                    break;
                case 2:
                    {
                        List<Field> fields = GetFields("fish", connection, false);
                        string query = "SELECT * FROM fish WHERE needs != has;";

                        MySqlCommand select = new MySqlCommand(query, connection);
                        MySqlDataReader selectReader = select.ExecuteReader();
                        List<(string, int)> output = new List<(string, int)>();
                        for(int field = 0; field < fields.Count; field++) {
                            output.Add((fields[field].name, fields[field].size));
                        }
                        output.Add(("\n\n", 0));

                        while(selectReader.Read()) {
                            for(int field = 0; field < selectReader.FieldCount; field++) {
                                output.Add((selectReader[field].ToString(), fields[field].size));
                            }
                            output.Add(("\n", 0));
                        }
                        DisplayValues(output, false);

                        select.Dispose();
                        selectReader.Close();
                    }
                    break;
                case 3:
                    {
                        List<Field> fields = GetFields("items", connection, false);
                        string query = "SELECT * FROM items WHERE needs != has;";

                        MySqlCommand select = new MySqlCommand(query, connection);
                        MySqlDataReader selectReader = select.ExecuteReader();
                        List<(string, int)> output = new List<(string, int)>();
                        for(int field = 0; field < fields.Count; field++) {
                            output.Add((fields[field].name, fields[field].size));
                        }
                        output.Add(("\n\n", 0));

                        while(selectReader.Read()) {
                            for(int field = 0; field < selectReader.FieldCount; field++) {
                                output.Add((selectReader[field].ToString(), fields[field].size));
                            }
                            output.Add(("\n", 0));
                        }
                        DisplayValues(output, false);

                        select.Dispose();
                        selectReader.Close();
                    }
                    break;
                case 4:
                    {
                        List<Field> fields = GetFields("plant", connection, false);
                        string query = "SELECT * FROM plant WHERE needs != has;";

                        MySqlCommand select = new MySqlCommand(query, connection);
                        MySqlDataReader selectReader = select.ExecuteReader();
                        List<(string, int)> output = new List<(string, int)>();
                        for(int field = 0; field < fields.Count; field++) {
                            output.Add((fields[field].name, fields[field].size));
                        }
                        output.Add(("\n\n", 0));

                        while(selectReader.Read()) {
                            for(int field = 0; field < selectReader.FieldCount; field++) {
                                output.Add((selectReader[field].ToString(), fields[field].size));
                            }
                            output.Add(("\n", 0));
                        }
                        DisplayValues(output, false);

                        select.Dispose();
                        selectReader.Close();
                    }
                    break;
                case 5:
                    return;
            }
        }

        // print values to console
        // bug: last line stays there even after scrolling. this is a bug in Console itself
        // workaround: change window height to make place for that line
        static void DisplayValues(List<(string, int)> output, bool autoContinue) {
            if(!autoContinue) {
                Console.BufferHeight = 3000;
                Console.BufferWidth = 300;
                Console.WindowHeight++;
                Console.Clear();
            }
            foreach((string value, int size) in output) {
                Console.Write(value.PadRight(size + 1));
            }

            if(!autoContinue) {
                Console.Write("\nNyomjon le egy billentyűt a folytatáshoz...");
                Console.ReadKey(true);
                Console.Clear();
                Console.WindowHeight--;
                Console.BufferHeight = Console.WindowHeight;
                Console.BufferWidth = Console.WindowWidth;
            }
        }

        static void Main() {
            // set encoding, title, etc. of Console
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Project 3";
            Console.CursorVisible = false;
            Console.SetWindowPosition(0, 0);
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;

            MySqlConnection connection = ConnectToDatabase();

            Menu(connection);

            connection.Close();

            Console.WriteLine("До свидания!");

            Console.ReadKey(true);
        }
    }
}
