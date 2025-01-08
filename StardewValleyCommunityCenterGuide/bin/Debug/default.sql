-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Mar 05, 2023 at 11:40 PM
-- Server version: 10.4.27-MariaDB
-- PHP Version: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `stardew_valley`
--

-- --------------------------------------------------------

--
-- Table structure for table `bundles`
--

CREATE TABLE `bundles` (
  `BundleID` int(11) NOT NULL,
  `Name` varchar(25) DEFAULT NULL,
  `Reward` varchar(25) NOT NULL,
  `Needs` int(5) DEFAULT NULL,
  `Max` int(3) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Dumping data for table `bundles`
--

INSERT INTO `bundles` (`BundleID`, `Name`, `Reward`, `Needs`, `Max`) VALUES
(1, 'Spring Foraging Bundle', 'Spring Mixed Seeds', 4, 4),
(2, 'Summer Foraging Bundle', 'Summer Mixed Seeds', 4, 4),
(3, 'Fall Foraging Bundle', 'Fall Mixed Seeds', 4, 4),
(4, 'Winter Foraging Bundle', 'Winter Mixed Seeds', 4, 4),
(5, 'Exotic Foraging Bundle', 'Autumn Bounty', 5, 5),
(6, 'Construction Bundle', 'Charcoal Kiln', 4, 4),
(7, 'Spring Crops Bundle', 'Speed-Gro', 4, 4),
(8, 'Summer Crops Bundle', 'Quality Sprinkler', 4, 4),
(9, 'Fall Crops Bundle', 'Bee House', 4, 4),
(10, 'Quality Crops Bundle', 'Preserves Jar', 4, 4),
(11, 'Animal Bundle', 'Cheese Press', 5, 6),
(12, 'Artisan Bundle', 'Keg', 6, 10),
(13, 'River Fish Bundle', 'Bait', 4, 4),
(14, 'Night Fishing Bundle', 'Glow Ring', 3, 3),
(15, 'Lake Fish Bundle', 'Lure', 4, 4),
(16, 'Ocean Fish Bundle', 'Beach Totems', 4, 4),
(17, 'Specialty Fish Bundle', 'Dish \'o the Sea', 4, 4),
(18, 'Crab Pot Bundle', 'Crab Pot', 5, 10),
(19, 'Adventurer\'s Bundle', 'Small Magnet Ring', 2, 4),
(20, 'Blacksmith\'s Bundle', 'Furnace', 3, 3),
(21, 'Geologist\'s Bundle', 'Omni Geode', 4, 4),
(22, 'Chef\'s Bundle', 'Pink Cake', 6, 6),
(23, 'Dye Bundle', 'Seed Maker', 6, 6),
(24, 'Field Research Bundle', 'Recycling Machine', 4, 4),
(25, 'Enchanter\'s Bundle', 'Gold Bar', 4, 4),
(26, 'Fodder Bundle', 'Heater', 3, 3);

-- --------------------------------------------------------

--
-- Table structure for table `fish`
--

CREATE TABLE `fish` (
  `name` varchar(20) NOT NULL,
  `spring` bit(6) DEFAULT NULL,
  `summer` bit(6) DEFAULT NULL,
  `fall` bit(6) DEFAULT NULL,
  `winter` bit(6) DEFAULT NULL,
  `place` varchar(50) DEFAULT NULL,
  `weather` varchar(10) DEFAULT NULL,
  `time` varchar(15) DEFAULT NULL,
  `has` int(3) NOT NULL DEFAULT 0,
  `needs` int(5) NOT NULL DEFAULT 0,
  `BundleID` int(3) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Dumping data for table `fish`
--

INSERT INTO `fish` (`name`, `spring`, `summer`, `fall`, `winter`, `place`, `weather`, `time`, `has`, `needs`, `BundleID`) VALUES
('Bream', b'000001', b'000001', b'000001', b'000001', 'River', 'any', '6pm-2am', 0, 0, 14),
('Bullhead', b'000001', b'000001', b'000001', b'000001', 'Mountain Lake', 'any', 'any', 0, 0, 15),
('Carp', b'000001', b'000001', b'000001', b'000001', 'Mountain Lake/Secret Woods', 'any', 'any', 0, 0, 15),
('Catfish', b'000001', b'000001', b'000001', b'000000', 'River/Secret Woods', 'rainy', '6am-midnight', 0, 0, 13),
('Clam', b'000001', b'000001', b'000001', b'000001', 'Ocean', 'any', 'any', 0, 0, 18),
('Cockle', b'000001', b'000001', b'000001', b'000001', 'Ocean', 'any', 'any', 0, 0, 18),
('Crab', b'000001', b'000001', b'000001', b'000001', 'Ocean', 'any', 'any', 0, 0, 18),
('Crayfish', b'000001', b'000001', b'000001', b'000001', 'River/Lake', 'any', 'any', 0, 0, 18),
('Eel', b'000001', b'000000', b'000001', b'000000', 'Ocean', 'rainy', '4pm-2am', 0, 0, 14),
('Ghostfish', b'000001', b'000001', b'000001', b'000001', 'The Mines (20-60)', 'any', 'any', 0, 0, 17),
('Largemouth Bass', b'000001', b'000001', b'000001', b'000001', 'Mountain Lake', 'any', '6am-7pm', 0, 0, 15),
('Lobster', b'000001', b'000001', b'000001', b'000001', 'Ocean', 'any', 'any', 0, 0, 18),
('Mussle', b'000001', b'000001', b'000001', b'000001', 'Ocean', 'any', 'any', 0, 0, 18),
('Oyster', b'000001', b'000001', b'000001', b'000001', 'Ocean', 'any', 'any', 0, 0, 18),
('Periwinkle', b'000001', b'000001', b'000001', b'000001', 'River/Lake', 'any', 'any', 0, 0, 18),
('Pufferfish', b'000000', b'000001', b'000000', b'000000', 'Ocean', 'sunny', '12pm-4pm', 0, 0, 17),
('Red Snapper', b'000000', b'000001', b'000001', b'000000', 'Ocean', 'rainy', '6am-7pm', 0, 0, 16),
('Sandfish', b'000001', b'000001', b'000001', b'000001', 'The Desert', 'any', '6am-8pm', 0, 0, 17),
('Sardine', b'000001', b'000000', b'000001', b'000001', 'Ocean', 'any', '6am-7pm', 0, 0, 16),
('Shad', b'000001', b'000001', b'000001', b'000000', 'River', 'rainy', '9am-2am', 0, 0, 13),
('Shrimp', b'000001', b'000001', b'000001', b'000001', 'Ocean', 'any', 'any', 0, 0, 18),
('Snail', b'000001', b'000001', b'000001', b'000001', 'River/Lake', 'any', 'any', 0, 0, 18),
('Sturgeon', b'000000', b'000001', b'000000', b'000001', 'Mountain Lake', 'any', 'any', 0, 0, 15),
('Sunfish', b'000001', b'000001', b'000000', b'000000', 'River', 'sunny', '6am-7pm', 0, 0, 13),
('Tiger Trout', b'000000', b'000000', b'000001', b'000001', 'River', 'any', '6am-7pm', 0, 0, 13),
('Tilapia', b'000000', b'000001', b'000001', b'000000', 'Ocean', 'any', '6am-2pm', 0, 0, 16),
('Tuna', b'000000', b'000001', b'000000', b'000001', 'Ocean', 'any', '6am-7pm', 0, 0, 16),
('Walleye', b'000000', b'000000', b'000001', b'000000', 'River/Mountain Lake/Cindersap Forest Pond', 'rainy', '12pm-2am', 0, 0, 14),
('Woodskip', b'000001', b'000001', b'000001', b'000001', 'Secret Woods', 'any', 'any', 0, 0, 17);

-- --------------------------------------------------------

--
-- Table structure for table `items`
--

CREATE TABLE `items` (
  `name` varchar(20) NOT NULL,
  `category` varchar(20) DEFAULT NULL,
  `preItem` varchar(20) DEFAULT NULL,
  `getFrom` varchar(30) DEFAULT NULL,
  `has` int(3) NOT NULL DEFAULT 0,
  `needs` int(5) NOT NULL DEFAULT 0,
  `BundleID` int(3) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Dumping data for table `items`
--

INSERT INTO `items` (`name`, `category`, `preItem`, `getFrom`, `has`, `needs`, `BundleID`) VALUES
('Aquamarine', 'Mineral', NULL, 'Mines', 0, 0, 23),
('Bat Wing', 'Monster Loot', NULL, 'Bat', 0, 0, 19),
('Cheese', 'Artisan Goods', 'Cheese Press', 'Milk', 0, 0, 12),
('Cloth', 'Artisan Goods', 'Loom', 'Wool', 0, 0, 12),
('Copper Bar', 'Resource', 'Furnace', 'Copper Ore', 0, 0, 20),
('Duck Egg', 'Animal Product', 'Big Coop', 'Duck', 0, 0, 11),
('Duck Feather', 'Animal Product', 'Big Coop', 'Duck', 0, 0, 23),
('Earth Crystal', 'Foraged Minerals', NULL, 'Mines', 0, 0, 21),
('Fire Quartz', 'Foraged Minerals', NULL, 'Mines', 0, 0, 21),
('Fried Egg', 'Cooking', 'Kitchen', 'Egg', 0, 0, 22),
('Frozen Geode', 'Mineral', NULL, 'Mines', 0, 0, 24),
('Frozen Tear', 'Foraged Minerals', NULL, 'Mines', 0, 0, 21),
('Goat Cheese', 'Artisan Goods', 'Cheese Press', 'Goat Milk', 0, 0, 12),
('Gold Bar', 'Resource', 'Furnace', 'Gold Ore', 0, 0, 20),
('Honey', 'Artisan Goods', 'Bee House', NULL, 0, 0, 12),
('Iron Bar', 'Resource', 'Furnace', 'Iron Ore', 0, 0, 20),
('Jelly', 'Artisan Goods', 'Preserves Jar', 'Fruit', 0, 0, 12),
('Lagre Milk', 'Animal Product', 'Barn', 'Cow', 0, 0, 11),
('Large Brown Egg', 'Animal Product', 'Coop', 'Chicken', 0, 0, 11),
('Large Egg', 'Animal Product', 'Coop', 'Chicken', 0, 0, 11),
('Large Goat Milk', 'Animal Product', 'Big Barn', 'Goat', 0, 0, 11),
('Maki Roll', 'Cooking', 'Kitchen', 'Seaweed,Rice,Fish', 0, 0, 22),
('Maple Syrup', 'Artisan Goods', 'Tapper', 'Maple Tree', 0, 0, 22),
('Nautilus Shell', 'Forage', NULL, 'Beach', 0, 0, 24),
('Oak Resin', 'Artisan Goods', 'Tapper', 'Oak Tree', 0, 0, 25),
('Quartz', 'Foraged Minerals', NULL, 'Mines', 0, 0, 21),
('Rabbit\'s Foot', 'Animal Product', 'Deluxe Coop', 'Rabbit', 0, 0, 25),
('Slime', 'Monster Loot', NULL, 'Slime', 0, 0, 19),
('Solar Essence', 'Monster Loot', NULL, 'Ghost/Squid Kid/Metal Head', 0, 0, 19),
('Truffle', 'Animal Product', 'Deluxe Barn', 'Pig', 0, 0, 22),
('Truffle Oil', 'Artisan Goods', 'Oil Maker', 'Truffle', 0, 0, 12),
('Void Essence', 'Monster Loot', NULL, 'Shadow Brute', 0, 0, 19),
('Wine', 'Artisan Goods', 'Keg', 'Fruit', 0, 0, 25),
('Wool', 'Animal Product', 'Deluxe Barn', 'Sheep', 0, 0, 11);

-- --------------------------------------------------------

--
-- Table structure for table `plant`
--

CREATE TABLE `plant` (
  `name` varchar(20) NOT NULL,
  `spring` bit(6) DEFAULT NULL,
  `summer` bit(6) DEFAULT NULL,
  `fall` bit(6) DEFAULT NULL,
  `winter` bit(6) DEFAULT NULL,
  `category` varchar(10) DEFAULT NULL,
  `toGrow` int(6) DEFAULT NULL,
  `toFind` varchar(20) DEFAULT NULL,
  `has` int(3) NOT NULL DEFAULT 0,
  `needs` int(5) NOT NULL DEFAULT 0,
  `BundleID` int(3) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Dumping data for table `plant`
--

INSERT INTO `plant` (`name`, `spring`, `summer`, `fall`, `winter`, `category`, `toGrow`, `toFind`, `has`, `needs`, `BundleID`) VALUES
('Apple', b'000000', b'000000', b'000001', b'000000', 'Fruit', 28, 'Pierre', 0, 0, 26),
('Apricot', b'000001', b'000000', b'000000', b'000000', 'Fruit', 28, 'Pierre', 0, 0, 12),
('Blackberry', b'000000', b'000000', b'000001', b'000000', 'Fruit', NULL, 'Foraging', 0, 0, 3),
('Blueberry', b'000000', b'000001', b'000000', b'000000', 'Fruit', 13, 'Pierre', 0, 0, 8),
('Cactus Fruit', b'000001', b'000001', b'000001', b'000001', 'Fruit', 12, 'Foraging', 0, 0, 5),
('Cauliflower', b'000001', b'000000', b'000000', b'000000', 'Vegetable', 12, 'Pierre', 0, 0, 7),
('Cave Carrot', b'000001', b'000001', b'000001', b'000001', 'Forage', NULL, 'Foraging', 0, 0, 5),
('Cherry', b'000001', b'000000', b'000000', b'000000', 'Fruit', 28, 'Pierre', 0, 0, 12),
('Coconut', b'000001', b'000001', b'000001', b'000001', 'Fruit', NULL, 'Foraging', 0, 0, 5),
('Common Mushroom', b'000000', b'000000', b'000001', b'000000', 'Forage', NULL, 'Foraging', 0, 0, 3),
('Corn', b'000000', b'000001', b'000001', b'000000', 'Vegetable', 14, 'Pierre', 0, 0, 9),
('Crocus', b'000000', b'000000', b'000000', b'000001', 'Flower', NULL, 'Foraging', 0, 0, 4),
('Crystal fruit', b'000000', b'000000', b'000000', b'000001', 'Forage', NULL, 'Foraging', 0, 0, 4),
('Daffodil', b'000001', b'000000', b'000000', b'000000', 'Forage', NULL, 'Foraging', 0, 0, 1),
('Dandelion', b'000001', b'000000', b'000000', b'000000', 'Forage', NULL, 'Foraging', 0, 0, 1),
('Eggplant', b'000000', b'000000', b'000001', b'000000', 'Vegetable', 5, 'Pierre', 0, 0, 9),
('Fern', b'000000', b'000001', b'000000', b'000000', 'Forage', NULL, 'Foraging', 0, 0, 22),
('Gold Corn', b'000000', b'000001', b'000001', b'000000', 'Vegetable', 14, 'Pierre', 0, 0, 10),
('Gold Melon', b'000000', b'000001', b'000000', b'000000', 'Fruit', 12, 'Pierre', 0, 0, 10),
('Gold Parsnip', b'000001', b'000000', b'000000', b'000000', 'Vegetable', 4, 'Pierre', 0, 0, 10),
('Gold Pumpkin', b'000000', b'000000', b'000001', b'000000', 'Vegetable', 13, 'Pierre', 0, 0, 10),
('Grape', b'000000', b'000001', b'000001', b'000000', 'Fruit', 10, 'Foraging/Pierre', 0, 0, 2),
('Green Bean', b'000001', b'000000', b'000000', b'000000', 'Vegetable', 10, 'Pierre', 0, 0, 7),
('Hay', b'000001', b'000001', b'000001', b'000001', 'Forage', NULL, 'Grass', 0, 0, 26),
('Hazelnut', b'000000', b'000000', b'000001', b'000000', 'Forage', NULL, 'Foraging', 0, 0, 3),
('Hot Pepper', b'000000', b'000001', b'000000', b'000000', 'Vegetable', 5, 'Pierre', 0, 0, 8),
('Leek', b'000001', b'000000', b'000000', b'000000', 'Forage', NULL, 'Foraging', 0, 0, 1),
('Melon', b'000000', b'000001', b'000000', b'000000', 'Fruit', 12, 'Pierre', 0, 0, 8),
('Morel', b'000001', b'000000', b'000000', b'000000', 'Forage', NULL, 'Foraging', 0, 0, 5),
('Orange', b'000000', b'000001', b'000000', b'000000', 'Fruit', 28, 'Pierre', 0, 0, 12),
('Parsnip', b'000001', b'000000', b'000000', b'000000', 'Vegetable', 4, 'Pierre', 0, 0, 7),
('Peach', b'000000', b'000001', b'000000', b'000000', 'Fruit', 28, 'Pierre', 0, 0, 12),
('Pomegranate', b'000000', b'000000', b'000001', b'000000', 'Fruit', 28, 'Pierre', 0, 0, 25),
('Poppy', b'000000', b'000001', b'000000', b'000000', 'Flower', 7, 'Pierre', 0, 0, 22),
('Potato', b'000001', b'000000', b'000000', b'000000', 'Vegetable', 6, 'Pierre', 0, 0, 7),
('Pumpkin', b'000000', b'000000', b'000001', b'000000', 'Vegetable', 13, 'Pierre', 0, 0, 9),
('Purple Mushroom', b'000001', b'000001', b'000001', b'000001', 'Forage', NULL, 'Foraging', 0, 0, 24),
('Red Cabbage', b'000000', b'000001', b'000000', b'000000', 'Vegetable', 9, 'Pierre', 0, 0, 23),
('Red Mushroom', b'000001', b'000001', b'000001', b'000001', 'Forage', NULL, 'Foraging', 0, 0, 23),
('Sea Urchin', b'000001', b'000001', b'000001', b'000001', 'Forage', NULL, 'Beach', 0, 0, 23),
('Snow Yam', b'000000', b'000000', b'000000', b'000001', 'Forage', NULL, 'Foraging', 0, 0, 4),
('Spice Berry', b'000000', b'000001', b'000000', b'000000', 'Fruit', NULL, 'Foraging', 0, 0, 2),
('Sunflower', b'000000', b'000001', b'000000', b'000000', 'Flower', 8, 'Pierre', 0, 0, 23),
('Sweet Pea', b'000000', b'000001', b'000000', b'000000', 'Flower', NULL, 'Foraging', 0, 0, 2),
('Tomato', b'000000', b'000001', b'000000', b'000000', 'Vegetable', 11, 'Pierre', 0, 0, 8),
('Wheat', b'000000', b'000001', b'000001', b'000000', 'Vegetable', 4, 'Pierre', 0, 0, 26),
('Wild Horseradish', b'000001', b'000000', b'000000', b'000000', 'Forage', NULL, 'Foraging', 0, 0, 1),
('Wild Plum', b'000000', b'000000', b'000001', b'000000', 'Fruit', NULL, 'Foraging', 0, 0, 3),
('Winter Root', b'000000', b'000000', b'000000', b'000001', 'Forage', NULL, 'Foraging', 0, 0, 4),
('Yam', b'000000', b'000000', b'000001', b'000000', 'Vegetable', 10, 'Pierre', 0, 0, 9);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `bundles`
--
ALTER TABLE `bundles`
  ADD PRIMARY KEY (`BundleID`);

--
-- Indexes for table `fish`
--
ALTER TABLE `fish`
  ADD PRIMARY KEY (`name`),
  ADD KEY `BundleID` (`BundleID`);

--
-- Indexes for table `items`
--
ALTER TABLE `items`
  ADD PRIMARY KEY (`name`),
  ADD KEY `BundleID` (`BundleID`);

--
-- Indexes for table `plant`
--
ALTER TABLE `plant`
  ADD PRIMARY KEY (`name`),
  ADD KEY `BundleID` (`BundleID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `bundles`
--
ALTER TABLE `bundles`
  MODIFY `BundleID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `fish`
--
ALTER TABLE `fish`
  ADD CONSTRAINT `fish_ibfk_1` FOREIGN KEY (`BundleID`) REFERENCES `bundles` (`BundleID`) ON DELETE CASCADE;

--
-- Constraints for table `items`
--
ALTER TABLE `items`
  ADD CONSTRAINT `items_ibfk_1` FOREIGN KEY (`BundleID`) REFERENCES `bundles` (`BundleID`) ON DELETE CASCADE;

--
-- Constraints for table `plant`
--
ALTER TABLE `plant`
  ADD CONSTRAINT `plant_ibfk_1` FOREIGN KEY (`BundleID`) REFERENCES `bundles` (`BundleID`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
