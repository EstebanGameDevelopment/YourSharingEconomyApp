-- phpMyAdmin SQL Dump
-- version 4.7.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 27, 2018 at 10:49 AM
-- Server version: 10.1.25-MariaDB
-- PHP Version: 7.1.7

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `yoursharingeconomyapp`
--

-- --------------------------------------------------------

--
-- Table structure for table `images`
--

CREATE TABLE `images` (
  `id` bigint(20) NOT NULL,
  `tabla` varchar(20) NOT NULL,
  `idorigin` bigint(20) NOT NULL,
  `size` int(11) NOT NULL,
  `data` mediumblob NOT NULL,
  `creation` bigint(20) NOT NULL,
  `type` int(11) NOT NULL,
  `url` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `proposals`
--

CREATE TABLE `proposals` (
  `id` bigint(20) NOT NULL,
  `user` bigint(20) NOT NULL,
  `request` bigint(20) NOT NULL,
  `type` int(11) NOT NULL,
  `title` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `description` varchar(2000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `price` int(11) NOT NULL,
  `deadline` bigint(20) NOT NULL,
  `accepted` int(11) NOT NULL,
  `created` bigint(20) NOT NULL,
  `active` int(11) NOT NULL,
  `reported` varchar(200) NOT NULL,
  `confirmedreported` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- --------------------------------------------------------

--
-- Table structure for table `requests`
--

CREATE TABLE `requests` (
  `id` bigint(20) NOT NULL,
  `customer` bigint(20) NOT NULL,
  `provider` bigint(20) NOT NULL,
  `title` varchar(500) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `images` int(11) NOT NULL,
  `referenceimg` bigint(20) NOT NULL,
  `village` varchar(200) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `mapdata` varchar(2000) NOT NULL,
  `latitude` float NOT NULL,
  `longitude` float NOT NULL,
  `price` int(11) NOT NULL,
  `currency` varchar(10) NOT NULL,
  `distance` int(11) NOT NULL,
  `flags` varchar(100) NOT NULL,
  `notifications` int(11) NOT NULL,
  `creationdate` bigint(20) NOT NULL,
  `deadline` bigint(20) NOT NULL,
  `score` int(11) NOT NULL,
  `deliverydate` bigint(20) NOT NULL,
  `workdays` int(11) NOT NULL,
  `proposal` bigint(20) NOT NULL,
  `feedbackcustomer` varchar(2000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `scorecustomer` int(11) NOT NULL,
  `feedbackprovider` varchar(2000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `scoreprovider` int(11) NOT NULL,
  `reported` varchar(200) NOT NULL,
  `flaged` int(11) NOT NULL,
  `confirmflag` int(11) NOT NULL,
  `signaturecustomer` varchar(1000) NOT NULL,
  `signatureprovider` varchar(1000) NOT NULL,
  `transactionid` varchar(1000) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` bigint(20) NOT NULL,
  `email` varchar(1000) NOT NULL,
  `validated` int(11) NOT NULL,
  `facebook` varchar(1000) NOT NULL,
  `password` varchar(400) NOT NULL,
  `name` varchar(30) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `village` varchar(200) NOT NULL,
  `mapdata` varchar(2000) NOT NULL,
  `latitude` float NOT NULL,
  `longitude` float NOT NULL,
  `friends` text CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `registerdate` bigint(20) NOT NULL,
  `lastlogin` bigint(20) NOT NULL,
  `rentstart` bigint(20) NOT NULL,
  `rentdays` int(11) NOT NULL,
  `scoreuser` int(11) NOT NULL,
  `scoreprovider` int(11) NOT NULL,
  `votesuser` int(11) NOT NULL,
  `votesprovider` int(11) NOT NULL,
  `resetpassword` bigint(20) NOT NULL,
  `resetcode` varchar(10) NOT NULL,
  `skills` varchar(2000) NOT NULL,
  `description` varchar(2000) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `additionalrequest` int(11) NOT NULL,
  `additionaloffer` int(11) NOT NULL,
  `purchasecode` varchar(256) NOT NULL,
  `ipaddress` varchar(200) NOT NULL,
  `banned` int(11) NOT NULL,
  `publickey` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `images`
--
ALTER TABLE `images`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `proposals`
--
ALTER TABLE `proposals`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `requests`
--
ALTER TABLE `requests`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
