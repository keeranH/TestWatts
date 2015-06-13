ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [Econocom], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

