-- GameMySQL
CREATE DATABASE IF NOT EXISTS GameMySQL;
USE GameMySQL;

CREATE TABLE IF NOT EXISTS UserGameData (
    Email VARCHAR(50) NOT NULL,
    Level INT NOT NULL DEFAULT 1,
    Exp INT NOT NULL DEFAULT 0,
    Win INT NOT NULL DEFAULT 0,
    Lose INT NOT NULL DEFAULT 0,
    PRIMARY KEY (Email),
    CONSTRAINT CHK_Level CHECK (Level BETWEEN 1 AND 100),
    CONSTRAINT CHK_Exp CHECK (Exp BETWEEN 0 AND 100000),
    CONSTRAINT CHK_Win CHECK (Win BETWEEN 0 AND 100000),
    CONSTRAINT CHK_Lose CHECK (Lose BETWEEN 0 AND 100000)
);

-- HiveMySQL
CREATE DATABASE IF NOT EXISTS HiveMySQL;
USE HiveMySQL;
CREATE TABLE IF NOT EXISTS Users (
    Email VARCHAR(50) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Salt VARCHAR(255) NOT NULL,
    Token VARCHAR(255) NOT NULL,
    PRIMARY KEY (Email)
);
