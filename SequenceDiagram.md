Hive Server & Game Server

```mermaEmail
sequenceDiagram
    actor client
    participant GameServer
    participant GameRedis
    participant GameMySQL
    participant HiveServer
    participant HiveRedis
    participant HiveMySQL


    client->>HiveServer: Create User Request(Email, Password)
    HiveServer->>HiveMySQL: User Data(Email, Password)
    HiveServer->>HiveRedis: Set Token(Email, Token)
    HiveServer-->>client: Create User Resposne(StatusCode)
    client->>HiveServer: Login Request(Email, Password)
    HiveServer->>HiveMySQL: Check Password
    HiveMySQL-->>HiveServer: Password
    alt Password mismatch
    HiveServer-->>client: Login Response(Fail)
    end
    HiveServer->>HiveRedis: Set Token(Email)
    HiveRedis-->>HiveServer: Token
    HiveServer-->>client: Login Response(Email, Token, StatusCode)
    client->>GameServer: Login Request(Email, Token)
    GameServer->>HiveServer: Validation(Email, Token)
    HiveServer->>HiveRedis: Get Token(Email, Token)
    HiveRedis-->>HiveServer: Token
    HiveServer-->>GameServer: AuthUser Response(Validity)
    alt Token mismatch
    GameServer-->>client: Login Response(Fail)
    end
    GameServer->>GameRedis: Set Token(Email, Token)
    GameServer->>GameMySQL: Get UserGameData
    GameMySQL-->>GameServer: UserGameData
    alt UserGameData not found
    GameServer->>GameMySQL: Create UserGameData
    end

