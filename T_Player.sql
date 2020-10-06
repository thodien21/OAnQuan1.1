-- Script Date: 09/04/2019 17:06  - ErikEJ.SqlCeScripting version 3.5.2.80
CREATE TABLE [T_Player] (
  [PlayerId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Pseudo] text NOT NULL
, [Password] text NOT NULL
, [FullName] text NOT NULL
, [IsAdmin] bigint NULL
, [IsEnabled] bigint NULL
, [WinGameQty] bigint NOT NULL
, [DrawGameQty] bigint NOT NULL
, [LoseGameQty] bigint NOT NULL
);
