-- Script Date: 22/04/2019 12:30  - ErikEJ.SqlCeScripting version 3.5.2.80
DROP TABLE [T_Board];
CREATE TABLE [T_Board] (
  [BoardId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [Turn] bigint NOT NULL
, [Player2Pseudo] text NULL
, [PlayerId] bigint NOT NULL
, CONSTRAINT [FK_T_Board_0_0] FOREIGN KEY ([PlayerId]) REFERENCES [T_Player] ([BoardId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
