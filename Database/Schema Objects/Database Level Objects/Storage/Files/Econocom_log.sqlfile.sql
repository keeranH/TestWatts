﻿ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [Econocom_log], FILENAME = '$(DefaultLogPath)$(DatabaseName)_log.ldf', MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

