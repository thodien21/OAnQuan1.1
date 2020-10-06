-- Script Date: 22/04/2019 12:28  - ErikEJ.SqlCeScripting version 3.5.2.80
DROP TABLE [T_Pool];
CREATE TABLE [T_Pool] (
  [PoolId] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
, [PlayerNumber] bigint NOT NULL
, [SmallTokenQty] bigint NOT NULL
, [BigTokenQty] bigint NOT NULL
, [PlayerId] bigint NOT NULL
, CONSTRAINT [FK_T_Pool_0_0] FOREIGN KEY ([PlayerId]) REFERENCES [T_Player] ([PoolId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
