-- Script Date: 22/04/2019 12:45  - ErikEJ.SqlCeScripting version 3.5.2.80
DROP TABLE [T_SquareList];
CREATE TABLE [T_SquareList] (
  [SquareId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [SquareIndex] bigint NOT NULL
, [SmallTokenQty] bigint NOT NULL
, [BigTokenQty] bigint NOT NULL
, [PlayerId] bigint NOT NULL
, CONSTRAINT [FK_T_SquareList_0_0] FOREIGN KEY ([PlayerId]) REFERENCES [T_Player] ([SquareId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
