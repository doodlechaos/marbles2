import { AwsClient } from "aws4fetch";
import { error, json } from "@sveltejs/kit";
import { AlgebraicType, deepEqual, DbConnectionImpl, DbConnectionBuilder, SubscriptionBuilderImpl, BinaryWriter } from "spacetimedb";
let _cached_ClockSchedule_type_value = null;
const ClockSchedule = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_ClockSchedule_type_value) return _cached_ClockSchedule_type_value;
    _cached_ClockSchedule_type_value = AlgebraicType.Product({ elements: [] });
    _cached_ClockSchedule_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U64 },
      { name: "scheduledAt", algebraicType: AlgebraicType.createScheduleAtType() }
    );
    return _cached_ClockSchedule_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, ClockSchedule.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, ClockSchedule.getTypeScriptAlgebraicType());
  }
};
let _cached_ClockUpdate_type_value = null;
const ClockUpdate = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_ClockUpdate_type_value) return _cached_ClockUpdate_type_value;
    _cached_ClockUpdate_type_value = AlgebraicType.Product({ elements: [] });
    _cached_ClockUpdate_type_value.value.elements.push(
      { name: "schedule", algebraicType: ClockSchedule.getTypeScriptAlgebraicType() }
    );
    return _cached_ClockUpdate_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, ClockUpdate.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, ClockUpdate.getTypeScriptAlgebraicType());
  }
};
let _cached_CloseAndCycleGameTile_type_value = null;
const CloseAndCycleGameTile = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_CloseAndCycleGameTile_type_value) return _cached_CloseAndCycleGameTile_type_value;
    _cached_CloseAndCycleGameTile_type_value = AlgebraicType.Product({ elements: [] });
    _cached_CloseAndCycleGameTile_type_value.value.elements.push(
      { name: "worldId", algebraicType: AlgebraicType.U8 }
    );
    return _cached_CloseAndCycleGameTile_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, CloseAndCycleGameTile.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, CloseAndCycleGameTile.getTypeScriptAlgebraicType());
  }
};
let _cached_Connect_type_value = null;
const Connect = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_Connect_type_value) return _cached_Connect_type_value;
    _cached_Connect_type_value = AlgebraicType.Product({ elements: [] });
    _cached_Connect_type_value.value.elements.push();
    return _cached_Connect_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, Connect.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, Connect.getTypeScriptAlgebraicType());
  }
};
let _cached_Disconnect_type_value = null;
const Disconnect = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_Disconnect_type_value) return _cached_Disconnect_type_value;
    _cached_Disconnect_type_value = AlgebraicType.Product({ elements: [] });
    _cached_Disconnect_type_value.value.elements.push();
    return _cached_Disconnect_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, Disconnect.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, Disconnect.getTypeScriptAlgebraicType());
  }
};
let _cached_IncrementPfpVersion_type_value = null;
const IncrementPfpVersion = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_IncrementPfpVersion_type_value) return _cached_IncrementPfpVersion_type_value;
    _cached_IncrementPfpVersion_type_value = AlgebraicType.Product({ elements: [] });
    _cached_IncrementPfpVersion_type_value.value.elements.push();
    return _cached_IncrementPfpVersion_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, IncrementPfpVersion.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, IncrementPfpVersion.getTypeScriptAlgebraicType());
  }
};
let _cached_SetUsername_type_value = null;
const SetUsername = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_SetUsername_type_value) return _cached_SetUsername_type_value;
    _cached_SetUsername_type_value = AlgebraicType.Product({ elements: [] });
    _cached_SetUsername_type_value.value.elements.push(
      { name: "username", algebraicType: AlgebraicType.String },
      { name: "overwriteExisting", algebraicType: AlgebraicType.Bool }
    );
    return _cached_SetUsername_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, SetUsername.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, SetUsername.getTypeScriptAlgebraicType());
  }
};
let _cached_Account_type_value = null;
const Account = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_Account_type_value) return _cached_Account_type_value;
    _cached_Account_type_value = AlgebraicType.Product({ elements: [] });
    _cached_Account_type_value.value.elements.push(
      { name: "identity", algebraicType: AlgebraicType.createIdentityType() },
      { name: "id", algebraicType: AlgebraicType.U64 },
      { name: "isConnected", algebraicType: AlgebraicType.Bool },
      { name: "marbles", algebraicType: AlgebraicType.U32 },
      { name: "points", algebraicType: AlgebraicType.U32 },
      { name: "gold", algebraicType: AlgebraicType.U32 },
      { name: "firstLoginBonusClaimed", algebraicType: AlgebraicType.Bool },
      { name: "isMember", algebraicType: AlgebraicType.Bool },
      { name: "dailyRewardClaimStreak", algebraicType: AlgebraicType.I64 },
      { name: "lastDailyRewardClaimDay", algebraicType: AlgebraicType.I64 }
    );
    return _cached_Account_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, Account.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, Account.getTypeScriptAlgebraicType());
  }
};
let _cached_UpsertAccount_type_value = null;
const UpsertAccount = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_UpsertAccount_type_value) return _cached_UpsertAccount_type_value;
    _cached_UpsertAccount_type_value = AlgebraicType.Product({ elements: [] });
    _cached_UpsertAccount_type_value.value.elements.push(
      { name: "row", algebraicType: Account.getTypeScriptAlgebraicType() }
    );
    return _cached_UpsertAccount_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, UpsertAccount.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, UpsertAccount.getTypeScriptAlgebraicType());
  }
};
let _cached_AccountSeq_type_value = null;
const AccountSeq = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_AccountSeq_type_value) return _cached_AccountSeq_type_value;
    _cached_AccountSeq_type_value = AlgebraicType.Product({ elements: [] });
    _cached_AccountSeq_type_value.value.elements.push(
      { name: "idS", algebraicType: AlgebraicType.U64 },
      { name: "seq", algebraicType: AlgebraicType.U64 }
    );
    return _cached_AccountSeq_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, AccountSeq.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, AccountSeq.getTypeScriptAlgebraicType());
  }
};
let _cached_UpsertAccountSeq_type_value = null;
const UpsertAccountSeq = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_UpsertAccountSeq_type_value) return _cached_UpsertAccountSeq_type_value;
    _cached_UpsertAccountSeq_type_value = AlgebraicType.Product({ elements: [] });
    _cached_UpsertAccountSeq_type_value.value.elements.push(
      { name: "row", algebraicType: AccountSeq.getTypeScriptAlgebraicType() }
    );
    return _cached_UpsertAccountSeq_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, UpsertAccountSeq.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, UpsertAccountSeq.getTypeScriptAlgebraicType());
  }
};
let _cached_InputFrame_type_value = null;
const InputFrame = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_InputFrame_type_value) return _cached_InputFrame_type_value;
    _cached_InputFrame_type_value = AlgebraicType.Product({ elements: [] });
    _cached_InputFrame_type_value.value.elements.push(
      { name: "seq", algebraicType: AlgebraicType.U16 },
      { name: "inputEventsList", algebraicType: AlgebraicType.Array(AlgebraicType.U8) }
    );
    return _cached_InputFrame_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, InputFrame.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, InputFrame.getTypeScriptAlgebraicType());
  }
};
let _cached_AuthFrame_type_value = null;
const AuthFrame = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_AuthFrame_type_value) return _cached_AuthFrame_type_value;
    _cached_AuthFrame_type_value = AlgebraicType.Product({ elements: [] });
    _cached_AuthFrame_type_value.value.elements.push(
      { name: "seq", algebraicType: AlgebraicType.U16 },
      { name: "frames", algebraicType: AlgebraicType.Array(InputFrame.getTypeScriptAlgebraicType()) }
    );
    return _cached_AuthFrame_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, AuthFrame.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, AuthFrame.getTypeScriptAlgebraicType());
  }
};
let _cached_UpsertAuthFrame_type_value = null;
const UpsertAuthFrame = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_UpsertAuthFrame_type_value) return _cached_UpsertAuthFrame_type_value;
    _cached_UpsertAuthFrame_type_value = AlgebraicType.Product({ elements: [] });
    _cached_UpsertAuthFrame_type_value.value.elements.push(
      { name: "row", algebraicType: AuthFrame.getTypeScriptAlgebraicType() }
    );
    return _cached_UpsertAuthFrame_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, UpsertAuthFrame.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, UpsertAuthFrame.getTypeScriptAlgebraicType());
  }
};
let _cached_BaseCfg_type_value = null;
const BaseCfg = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_BaseCfg_type_value) return _cached_BaseCfg_type_value;
    _cached_BaseCfg_type_value = AlgebraicType.Product({ elements: [] });
    _cached_BaseCfg_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U8 },
      { name: "clockIntervalSec", algebraicType: AlgebraicType.F64 },
      { name: "targetStepsPerSecond", algebraicType: AlgebraicType.U16 },
      { name: "physicsStepsPerBatch", algebraicType: AlgebraicType.U16 },
      { name: "stepsPerAuthFrame", algebraicType: AlgebraicType.U16 },
      { name: "authFrameTimeErrorThresholdSec", algebraicType: AlgebraicType.F64 },
      { name: "logInputFrameTimes", algebraicType: AlgebraicType.Bool },
      { name: "logAuthFrameTimeDiffs", algebraicType: AlgebraicType.Bool },
      { name: "gcCacheAccountTimeoutMinutes", algebraicType: AlgebraicType.F64 }
    );
    return _cached_BaseCfg_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, BaseCfg.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, BaseCfg.getTypeScriptAlgebraicType());
  }
};
let _cached_UpsertBaseCfg_type_value = null;
const UpsertBaseCfg = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_UpsertBaseCfg_type_value) return _cached_UpsertBaseCfg_type_value;
    _cached_UpsertBaseCfg_type_value = AlgebraicType.Product({ elements: [] });
    _cached_UpsertBaseCfg_type_value.value.elements.push(
      { name: "row", algebraicType: BaseCfg.getTypeScriptAlgebraicType() }
    );
    return _cached_UpsertBaseCfg_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, UpsertBaseCfg.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, UpsertBaseCfg.getTypeScriptAlgebraicType());
  }
};
let _cached_InputCollector_type_value = null;
const InputCollector = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_InputCollector_type_value) return _cached_InputCollector_type_value;
    _cached_InputCollector_type_value = AlgebraicType.Product({ elements: [] });
    _cached_InputCollector_type_value.value.elements.push(
      { name: "delaySeqs", algebraicType: AlgebraicType.U16 },
      { name: "inputEventData", algebraicType: AlgebraicType.Array(AlgebraicType.U8) }
    );
    return _cached_InputCollector_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, InputCollector.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, InputCollector.getTypeScriptAlgebraicType());
  }
};
let _cached_UpsertInputCollector_type_value = null;
const UpsertInputCollector = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_UpsertInputCollector_type_value) return _cached_UpsertInputCollector_type_value;
    _cached_UpsertInputCollector_type_value = AlgebraicType.Product({ elements: [] });
    _cached_UpsertInputCollector_type_value.value.elements.push(
      { name: "row", algebraicType: InputCollector.getTypeScriptAlgebraicType() }
    );
    return _cached_UpsertInputCollector_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, UpsertInputCollector.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, UpsertInputCollector.getTypeScriptAlgebraicType());
  }
};
let _cached_UpsertInputFrame_type_value = null;
const UpsertInputFrame = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_UpsertInputFrame_type_value) return _cached_UpsertInputFrame_type_value;
    _cached_UpsertInputFrame_type_value = AlgebraicType.Product({ elements: [] });
    _cached_UpsertInputFrame_type_value.value.elements.push(
      { name: "row", algebraicType: InputFrame.getTypeScriptAlgebraicType() }
    );
    return _cached_UpsertInputFrame_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, UpsertInputFrame.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, UpsertInputFrame.getTypeScriptAlgebraicType());
  }
};
let _cached_LevelFileData_type_value = null;
const LevelFileData = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_LevelFileData_type_value) return _cached_LevelFileData_type_value;
    _cached_LevelFileData_type_value = AlgebraicType.Product({ elements: [] });
    _cached_LevelFileData_type_value.value.elements.push(
      { name: "unityPrefabGuid", algebraicType: AlgebraicType.String },
      { name: "levelFileBinary", algebraicType: AlgebraicType.Array(AlgebraicType.U8) }
    );
    return _cached_LevelFileData_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, LevelFileData.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, LevelFileData.getTypeScriptAlgebraicType());
  }
};
let _cached_UpsertLevelFileData_type_value = null;
const UpsertLevelFileData = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_UpsertLevelFileData_type_value) return _cached_UpsertLevelFileData_type_value;
    _cached_UpsertLevelFileData_type_value = AlgebraicType.Product({ elements: [] });
    _cached_UpsertLevelFileData_type_value.value.elements.push(
      { name: "levelFileData", algebraicType: LevelFileData.getTypeScriptAlgebraicType() }
    );
    return _cached_UpsertLevelFileData_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, UpsertLevelFileData.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, UpsertLevelFileData.getTypeScriptAlgebraicType());
  }
};
class AccountTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `identity` unique index on the table `Account`,
   * which allows point queries on the field of the same name
   * via the [`AccountIdentityUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.account.identity().find(...)`.
   *
   * Get a handle on the `identity` unique index on the table `Account`.
   */
  identity = {
    // Find the subscribed row whose `identity` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.identity, col_val)) {
          return row;
        }
      }
    }
  };
  /**
   * Access to the `id` unique index on the table `Account`,
   * which allows point queries on the field of the same name
   * via the [`AccountIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.account.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `Account`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_AccountCustomization_type_value = null;
const AccountCustomization = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_AccountCustomization_type_value) return _cached_AccountCustomization_type_value;
    _cached_AccountCustomization_type_value = AlgebraicType.Product({ elements: [] });
    _cached_AccountCustomization_type_value.value.elements.push(
      { name: "accountId", algebraicType: AlgebraicType.U64 },
      { name: "username", algebraicType: AlgebraicType.String },
      { name: "pfpVersion", algebraicType: AlgebraicType.U8 }
    );
    return _cached_AccountCustomization_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, AccountCustomization.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, AccountCustomization.getTypeScriptAlgebraicType());
  }
};
class AccountCustomizationTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `accountId` unique index on the table `AccountCustomization`,
   * which allows point queries on the field of the same name
   * via the [`AccountCustomizationAccountIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.accountCustomization.accountId().find(...)`.
   *
   * Get a handle on the `accountId` unique index on the table `AccountCustomization`.
   */
  accountId = {
    // Find the subscribed row whose `accountId` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.accountId, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
class AccountSeqTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `idS` unique index on the table `AccountSeq`,
   * which allows point queries on the field of the same name
   * via the [`AccountSeqIdSUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.accountSeq.idS().find(...)`.
   *
   * Get a handle on the `idS` unique index on the table `AccountSeq`.
   */
  idS = {
    // Find the subscribed row whose `idS` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.idS, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_Admin_type_value = null;
const Admin = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_Admin_type_value) return _cached_Admin_type_value;
    _cached_Admin_type_value = AlgebraicType.Product({ elements: [] });
    _cached_Admin_type_value.value.elements.push(
      { name: "adminIdentity", algebraicType: AlgebraicType.createIdentityType() }
    );
    return _cached_Admin_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, Admin.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, Admin.getTypeScriptAlgebraicType());
  }
};
class AdminTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `adminIdentity` unique index on the table `Admin`,
   * which allows point queries on the field of the same name
   * via the [`AdminAdminIdentityUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.admin.adminIdentity().find(...)`.
   *
   * Get a handle on the `adminIdentity` unique index on the table `Admin`.
   */
  adminIdentity = {
    // Find the subscribed row whose `adminIdentity` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.adminIdentity, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
class AuthFrameTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `seq` unique index on the table `AuthFrame`,
   * which allows point queries on the field of the same name
   * via the [`AuthFrameSeqUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.authFrame.seq().find(...)`.
   *
   * Get a handle on the `seq` unique index on the table `AuthFrame`.
   */
  seq = {
    // Find the subscribed row whose `seq` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.seq, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
class BaseCfgTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `BaseCfg`,
   * which allows point queries on the field of the same name
   * via the [`BaseCfgIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.baseCfg.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `BaseCfg`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_Clock_type_value = null;
const Clock = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_Clock_type_value) return _cached_Clock_type_value;
    _cached_Clock_type_value = AlgebraicType.Product({ elements: [] });
    _cached_Clock_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U8 },
      { name: "prevClockUpdate", algebraicType: AlgebraicType.createTimestampType() },
      { name: "tickTimeAccumulatorSec", algebraicType: AlgebraicType.F32 }
    );
    return _cached_Clock_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, Clock.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, Clock.getTypeScriptAlgebraicType());
  }
};
class ClockTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `Clock`,
   * which allows point queries on the field of the same name
   * via the [`ClockIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.clock.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `Clock`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
class ClockScheduleTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `ClockSchedule`,
   * which allows point queries on the field of the same name
   * via the [`ClockScheduleIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.clockSchedule.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `ClockSchedule`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_DeterminismCheck_type_value = null;
const DeterminismCheck = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_DeterminismCheck_type_value) return _cached_DeterminismCheck_type_value;
    _cached_DeterminismCheck_type_value = AlgebraicType.Product({ elements: [] });
    _cached_DeterminismCheck_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U8 },
      { name: "seq", algebraicType: AlgebraicType.U16 },
      { name: "hashString", algebraicType: AlgebraicType.String }
    );
    return _cached_DeterminismCheck_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, DeterminismCheck.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, DeterminismCheck.getTypeScriptAlgebraicType());
  }
};
class DeterminismCheckTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `DeterminismCheck`,
   * which allows point queries on the field of the same name
   * via the [`DeterminismCheckIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.determinismCheck.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `DeterminismCheck`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_GameCoreSnap_type_value = null;
const GameCoreSnap = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_GameCoreSnap_type_value) return _cached_GameCoreSnap_type_value;
    _cached_GameCoreSnap_type_value = AlgebraicType.Product({ elements: [] });
    _cached_GameCoreSnap_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U8 },
      { name: "seq", algebraicType: AlgebraicType.U16 },
      { name: "binaryData", algebraicType: AlgebraicType.Array(AlgebraicType.U8) }
    );
    return _cached_GameCoreSnap_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, GameCoreSnap.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, GameCoreSnap.getTypeScriptAlgebraicType());
  }
};
class GameCoreSnapTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `GameCoreSnap`,
   * which allows point queries on the field of the same name
   * via the [`GameCoreSnapIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.gameCoreSnap.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `GameCoreSnap`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
class InputCollectorTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
}
class InputFrameTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `seq` unique index on the table `InputFrame`,
   * which allows point queries on the field of the same name
   * via the [`InputFrameSeqUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.inputFrame.seq().find(...)`.
   *
   * Get a handle on the `seq` unique index on the table `InputFrame`.
   */
  seq = {
    // Find the subscribed row whose `seq` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.seq, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_LastAuthFrameTimestamp_type_value = null;
const LastAuthFrameTimestamp = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_LastAuthFrameTimestamp_type_value) return _cached_LastAuthFrameTimestamp_type_value;
    _cached_LastAuthFrameTimestamp_type_value = AlgebraicType.Product({ elements: [] });
    _cached_LastAuthFrameTimestamp_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U8 },
      { name: "lastAuthFrameTime", algebraicType: AlgebraicType.createTimestampType() }
    );
    return _cached_LastAuthFrameTimestamp_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, LastAuthFrameTimestamp.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, LastAuthFrameTimestamp.getTypeScriptAlgebraicType());
  }
};
class LastAuthFrameTimestampTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `LastAuthFrameTimestamp`,
   * which allows point queries on the field of the same name
   * via the [`LastAuthFrameTimestampIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.lastAuthFrameTimestamp.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `LastAuthFrameTimestamp`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
class LevelFileDataTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `unityPrefabGuid` unique index on the table `LevelFileData`,
   * which allows point queries on the field of the same name
   * via the [`LevelFileDataUnityPrefabGuidUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.levelFileData.unityPrefabGuid().find(...)`.
   *
   * Get a handle on the `unityPrefabGuid` unique index on the table `LevelFileData`.
   */
  unityPrefabGuid = {
    // Find the subscribed row whose `unityPrefabGuid` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.unityPrefabGuid, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_Seq_type_value = null;
const Seq = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_Seq_type_value) return _cached_Seq_type_value;
    _cached_Seq_type_value = AlgebraicType.Product({ elements: [] });
    _cached_Seq_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U8 },
      { name: "value", algebraicType: AlgebraicType.U16 }
    );
    return _cached_Seq_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, Seq.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, Seq.getTypeScriptAlgebraicType());
  }
};
class SeqTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `Seq`,
   * which allows point queries on the field of the same name
   * via the [`SeqIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.seq.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `Seq`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_StepsSinceLastAuthFrame_type_value = null;
const StepsSinceLastAuthFrame = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_StepsSinceLastAuthFrame_type_value) return _cached_StepsSinceLastAuthFrame_type_value;
    _cached_StepsSinceLastAuthFrame_type_value = AlgebraicType.Product({ elements: [] });
    _cached_StepsSinceLastAuthFrame_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U8 },
      { name: "value", algebraicType: AlgebraicType.U16 }
    );
    return _cached_StepsSinceLastAuthFrame_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, StepsSinceLastAuthFrame.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, StepsSinceLastAuthFrame.getTypeScriptAlgebraicType());
  }
};
class StepsSinceLastAuthFrameTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `StepsSinceLastAuthFrame`,
   * which allows point queries on the field of the same name
   * via the [`StepsSinceLastAuthFrameIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.stepsSinceLastAuthFrame.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `StepsSinceLastAuthFrame`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
let _cached_StepsSinceLastBatch_type_value = null;
const StepsSinceLastBatch = {
  /**
  * A function which returns this type represented as an AlgebraicType.
  * This function is derived from the AlgebraicType used to generate this type.
  */
  getTypeScriptAlgebraicType() {
    if (_cached_StepsSinceLastBatch_type_value) return _cached_StepsSinceLastBatch_type_value;
    _cached_StepsSinceLastBatch_type_value = AlgebraicType.Product({ elements: [] });
    _cached_StepsSinceLastBatch_type_value.value.elements.push(
      { name: "id", algebraicType: AlgebraicType.U8 },
      { name: "value", algebraicType: AlgebraicType.U16 }
    );
    return _cached_StepsSinceLastBatch_type_value;
  },
  serialize(writer, value) {
    AlgebraicType.serializeValue(writer, StepsSinceLastBatch.getTypeScriptAlgebraicType(), value);
  },
  deserialize(reader) {
    return AlgebraicType.deserializeValue(reader, StepsSinceLastBatch.getTypeScriptAlgebraicType());
  }
};
class StepsSinceLastBatchTableHandle {
  // phantom type to track the table name
  tableName;
  tableCache;
  constructor(tableCache) {
    this.tableCache = tableCache;
  }
  count() {
    return this.tableCache.count();
  }
  iter() {
    return this.tableCache.iter();
  }
  /**
   * Access to the `id` unique index on the table `StepsSinceLastBatch`,
   * which allows point queries on the field of the same name
   * via the [`StepsSinceLastBatchIdUnique.find`] method.
   *
   * Users are encouraged not to explicitly reference this type,
   * but to directly chain method calls,
   * like `ctx.db.stepsSinceLastBatch.id().find(...)`.
   *
   * Get a handle on the `id` unique index on the table `StepsSinceLastBatch`.
   */
  id = {
    // Find the subscribed row whose `id` column value is equal to `col_val`,
    // if such a row is present in the client cache.
    find: (col_val) => {
      for (let row of this.tableCache.iter()) {
        if (deepEqual(row.id, col_val)) {
          return row;
        }
      }
    }
  };
  onInsert = (cb) => {
    return this.tableCache.onInsert(cb);
  };
  removeOnInsert = (cb) => {
    return this.tableCache.removeOnInsert(cb);
  };
  onDelete = (cb) => {
    return this.tableCache.onDelete(cb);
  };
  removeOnDelete = (cb) => {
    return this.tableCache.removeOnDelete(cb);
  };
  // Updates are only defined for tables with primary keys.
  onUpdate = (cb) => {
    return this.tableCache.onUpdate(cb);
  };
  removeOnUpdate = (cb) => {
    return this.tableCache.removeOnUpdate(cb);
  };
}
const REMOTE_MODULE = {
  tables: {
    Account: {
      tableName: "Account",
      rowType: Account.getTypeScriptAlgebraicType(),
      primaryKey: "identity",
      primaryKeyInfo: {
        colName: "identity",
        colType: Account.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    AccountCustomization: {
      tableName: "AccountCustomization",
      rowType: AccountCustomization.getTypeScriptAlgebraicType(),
      primaryKey: "accountId",
      primaryKeyInfo: {
        colName: "accountId",
        colType: AccountCustomization.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    AccountSeq: {
      tableName: "AccountSeq",
      rowType: AccountSeq.getTypeScriptAlgebraicType(),
      primaryKey: "idS",
      primaryKeyInfo: {
        colName: "idS",
        colType: AccountSeq.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    Admin: {
      tableName: "Admin",
      rowType: Admin.getTypeScriptAlgebraicType(),
      primaryKey: "adminIdentity",
      primaryKeyInfo: {
        colName: "adminIdentity",
        colType: Admin.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    AuthFrame: {
      tableName: "AuthFrame",
      rowType: AuthFrame.getTypeScriptAlgebraicType(),
      primaryKey: "seq",
      primaryKeyInfo: {
        colName: "seq",
        colType: AuthFrame.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    BaseCfg: {
      tableName: "BaseCfg",
      rowType: BaseCfg.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: BaseCfg.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    Clock: {
      tableName: "Clock",
      rowType: Clock.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: Clock.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    ClockSchedule: {
      tableName: "ClockSchedule",
      rowType: ClockSchedule.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: ClockSchedule.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    DeterminismCheck: {
      tableName: "DeterminismCheck",
      rowType: DeterminismCheck.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: DeterminismCheck.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    GameCoreSnap: {
      tableName: "GameCoreSnap",
      rowType: GameCoreSnap.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: GameCoreSnap.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    InputCollector: {
      tableName: "InputCollector",
      rowType: InputCollector.getTypeScriptAlgebraicType()
    },
    InputFrame: {
      tableName: "InputFrame",
      rowType: InputFrame.getTypeScriptAlgebraicType(),
      primaryKey: "seq",
      primaryKeyInfo: {
        colName: "seq",
        colType: InputFrame.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    LastAuthFrameTimestamp: {
      tableName: "LastAuthFrameTimestamp",
      rowType: LastAuthFrameTimestamp.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: LastAuthFrameTimestamp.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    LevelFileData: {
      tableName: "LevelFileData",
      rowType: LevelFileData.getTypeScriptAlgebraicType(),
      primaryKey: "unityPrefabGuid",
      primaryKeyInfo: {
        colName: "unityPrefabGuid",
        colType: LevelFileData.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    Seq: {
      tableName: "Seq",
      rowType: Seq.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: Seq.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    StepsSinceLastAuthFrame: {
      tableName: "StepsSinceLastAuthFrame",
      rowType: StepsSinceLastAuthFrame.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: StepsSinceLastAuthFrame.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    },
    StepsSinceLastBatch: {
      tableName: "StepsSinceLastBatch",
      rowType: StepsSinceLastBatch.getTypeScriptAlgebraicType(),
      primaryKey: "id",
      primaryKeyInfo: {
        colName: "id",
        colType: StepsSinceLastBatch.getTypeScriptAlgebraicType().value.elements[0].algebraicType
      }
    }
  },
  reducers: {
    ClockUpdate: {
      reducerName: "ClockUpdate",
      argsType: ClockUpdate.getTypeScriptAlgebraicType()
    },
    CloseAndCycleGameTile: {
      reducerName: "CloseAndCycleGameTile",
      argsType: CloseAndCycleGameTile.getTypeScriptAlgebraicType()
    },
    Connect: {
      reducerName: "Connect",
      argsType: Connect.getTypeScriptAlgebraicType()
    },
    Disconnect: {
      reducerName: "Disconnect",
      argsType: Disconnect.getTypeScriptAlgebraicType()
    },
    IncrementPfpVersion: {
      reducerName: "IncrementPfpVersion",
      argsType: IncrementPfpVersion.getTypeScriptAlgebraicType()
    },
    SetUsername: {
      reducerName: "SetUsername",
      argsType: SetUsername.getTypeScriptAlgebraicType()
    },
    UpsertAccount: {
      reducerName: "UpsertAccount",
      argsType: UpsertAccount.getTypeScriptAlgebraicType()
    },
    UpsertAccountSeq: {
      reducerName: "UpsertAccountSeq",
      argsType: UpsertAccountSeq.getTypeScriptAlgebraicType()
    },
    UpsertAuthFrame: {
      reducerName: "UpsertAuthFrame",
      argsType: UpsertAuthFrame.getTypeScriptAlgebraicType()
    },
    UpsertBaseCfg: {
      reducerName: "UpsertBaseCfg",
      argsType: UpsertBaseCfg.getTypeScriptAlgebraicType()
    },
    UpsertInputCollector: {
      reducerName: "UpsertInputCollector",
      argsType: UpsertInputCollector.getTypeScriptAlgebraicType()
    },
    UpsertInputFrame: {
      reducerName: "UpsertInputFrame",
      argsType: UpsertInputFrame.getTypeScriptAlgebraicType()
    },
    UpsertLevelFileData: {
      reducerName: "UpsertLevelFileData",
      argsType: UpsertLevelFileData.getTypeScriptAlgebraicType()
    }
  },
  versionInfo: {
    cliVersion: "1.8.0"
  },
  // Constructors which are used by the DbConnectionImpl to
  // extract type information from the generated RemoteModule.
  //
  // NOTE: This is not strictly necessary for `eventContextConstructor` because
  // all we do is build a TypeScript object which we could have done inside the
  // SDK, but if in the future we wanted to create a class this would be
  // necessary because classes have methods, so we'll keep it.
  eventContextConstructor: (imp, event) => {
    return {
      ...imp,
      event
    };
  },
  dbViewConstructor: (imp) => {
    return new RemoteTables(imp);
  },
  reducersConstructor: (imp, setReducerFlags) => {
    return new RemoteReducers(imp, setReducerFlags);
  },
  setReducerFlagsConstructor: () => {
    return new SetReducerFlags();
  }
};
class RemoteReducers {
  constructor(connection, setCallReducerFlags) {
    this.connection = connection;
    this.setCallReducerFlags = setCallReducerFlags;
  }
  clockUpdate(schedule) {
    const __args = { schedule };
    let __writer = new BinaryWriter(1024);
    ClockUpdate.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("ClockUpdate", __argsBuffer, this.setCallReducerFlags.clockUpdateFlags);
  }
  onClockUpdate(callback) {
    this.connection.onReducer("ClockUpdate", callback);
  }
  removeOnClockUpdate(callback) {
    this.connection.offReducer("ClockUpdate", callback);
  }
  closeAndCycleGameTile(worldId) {
    const __args = { worldId };
    let __writer = new BinaryWriter(1024);
    CloseAndCycleGameTile.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("CloseAndCycleGameTile", __argsBuffer, this.setCallReducerFlags.closeAndCycleGameTileFlags);
  }
  onCloseAndCycleGameTile(callback) {
    this.connection.onReducer("CloseAndCycleGameTile", callback);
  }
  removeOnCloseAndCycleGameTile(callback) {
    this.connection.offReducer("CloseAndCycleGameTile", callback);
  }
  onConnect(callback) {
    this.connection.onReducer("Connect", callback);
  }
  removeOnConnect(callback) {
    this.connection.offReducer("Connect", callback);
  }
  onDisconnect(callback) {
    this.connection.onReducer("Disconnect", callback);
  }
  removeOnDisconnect(callback) {
    this.connection.offReducer("Disconnect", callback);
  }
  incrementPfpVersion() {
    this.connection.callReducer("IncrementPfpVersion", new Uint8Array(0), this.setCallReducerFlags.incrementPfpVersionFlags);
  }
  onIncrementPfpVersion(callback) {
    this.connection.onReducer("IncrementPfpVersion", callback);
  }
  removeOnIncrementPfpVersion(callback) {
    this.connection.offReducer("IncrementPfpVersion", callback);
  }
  setUsername(username, overwriteExisting) {
    const __args = { username, overwriteExisting };
    let __writer = new BinaryWriter(1024);
    SetUsername.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("SetUsername", __argsBuffer, this.setCallReducerFlags.setUsernameFlags);
  }
  onSetUsername(callback) {
    this.connection.onReducer("SetUsername", callback);
  }
  removeOnSetUsername(callback) {
    this.connection.offReducer("SetUsername", callback);
  }
  upsertAccount(row) {
    const __args = { row };
    let __writer = new BinaryWriter(1024);
    UpsertAccount.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("UpsertAccount", __argsBuffer, this.setCallReducerFlags.upsertAccountFlags);
  }
  onUpsertAccount(callback) {
    this.connection.onReducer("UpsertAccount", callback);
  }
  removeOnUpsertAccount(callback) {
    this.connection.offReducer("UpsertAccount", callback);
  }
  upsertAccountSeq(row) {
    const __args = { row };
    let __writer = new BinaryWriter(1024);
    UpsertAccountSeq.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("UpsertAccountSeq", __argsBuffer, this.setCallReducerFlags.upsertAccountSeqFlags);
  }
  onUpsertAccountSeq(callback) {
    this.connection.onReducer("UpsertAccountSeq", callback);
  }
  removeOnUpsertAccountSeq(callback) {
    this.connection.offReducer("UpsertAccountSeq", callback);
  }
  upsertAuthFrame(row) {
    const __args = { row };
    let __writer = new BinaryWriter(1024);
    UpsertAuthFrame.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("UpsertAuthFrame", __argsBuffer, this.setCallReducerFlags.upsertAuthFrameFlags);
  }
  onUpsertAuthFrame(callback) {
    this.connection.onReducer("UpsertAuthFrame", callback);
  }
  removeOnUpsertAuthFrame(callback) {
    this.connection.offReducer("UpsertAuthFrame", callback);
  }
  upsertBaseCfg(row) {
    const __args = { row };
    let __writer = new BinaryWriter(1024);
    UpsertBaseCfg.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("UpsertBaseCfg", __argsBuffer, this.setCallReducerFlags.upsertBaseCfgFlags);
  }
  onUpsertBaseCfg(callback) {
    this.connection.onReducer("UpsertBaseCfg", callback);
  }
  removeOnUpsertBaseCfg(callback) {
    this.connection.offReducer("UpsertBaseCfg", callback);
  }
  upsertInputCollector(row) {
    const __args = { row };
    let __writer = new BinaryWriter(1024);
    UpsertInputCollector.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("UpsertInputCollector", __argsBuffer, this.setCallReducerFlags.upsertInputCollectorFlags);
  }
  onUpsertInputCollector(callback) {
    this.connection.onReducer("UpsertInputCollector", callback);
  }
  removeOnUpsertInputCollector(callback) {
    this.connection.offReducer("UpsertInputCollector", callback);
  }
  upsertInputFrame(row) {
    const __args = { row };
    let __writer = new BinaryWriter(1024);
    UpsertInputFrame.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("UpsertInputFrame", __argsBuffer, this.setCallReducerFlags.upsertInputFrameFlags);
  }
  onUpsertInputFrame(callback) {
    this.connection.onReducer("UpsertInputFrame", callback);
  }
  removeOnUpsertInputFrame(callback) {
    this.connection.offReducer("UpsertInputFrame", callback);
  }
  upsertLevelFileData(levelFileData) {
    const __args = { levelFileData };
    let __writer = new BinaryWriter(1024);
    UpsertLevelFileData.serialize(__writer, __args);
    let __argsBuffer = __writer.getBuffer();
    this.connection.callReducer("UpsertLevelFileData", __argsBuffer, this.setCallReducerFlags.upsertLevelFileDataFlags);
  }
  onUpsertLevelFileData(callback) {
    this.connection.onReducer("UpsertLevelFileData", callback);
  }
  removeOnUpsertLevelFileData(callback) {
    this.connection.offReducer("UpsertLevelFileData", callback);
  }
}
class SetReducerFlags {
  clockUpdateFlags = "FullUpdate";
  clockUpdate(flags) {
    this.clockUpdateFlags = flags;
  }
  closeAndCycleGameTileFlags = "FullUpdate";
  closeAndCycleGameTile(flags) {
    this.closeAndCycleGameTileFlags = flags;
  }
  incrementPfpVersionFlags = "FullUpdate";
  incrementPfpVersion(flags) {
    this.incrementPfpVersionFlags = flags;
  }
  setUsernameFlags = "FullUpdate";
  setUsername(flags) {
    this.setUsernameFlags = flags;
  }
  upsertAccountFlags = "FullUpdate";
  upsertAccount(flags) {
    this.upsertAccountFlags = flags;
  }
  upsertAccountSeqFlags = "FullUpdate";
  upsertAccountSeq(flags) {
    this.upsertAccountSeqFlags = flags;
  }
  upsertAuthFrameFlags = "FullUpdate";
  upsertAuthFrame(flags) {
    this.upsertAuthFrameFlags = flags;
  }
  upsertBaseCfgFlags = "FullUpdate";
  upsertBaseCfg(flags) {
    this.upsertBaseCfgFlags = flags;
  }
  upsertInputCollectorFlags = "FullUpdate";
  upsertInputCollector(flags) {
    this.upsertInputCollectorFlags = flags;
  }
  upsertInputFrameFlags = "FullUpdate";
  upsertInputFrame(flags) {
    this.upsertInputFrameFlags = flags;
  }
  upsertLevelFileDataFlags = "FullUpdate";
  upsertLevelFileData(flags) {
    this.upsertLevelFileDataFlags = flags;
  }
}
class RemoteTables {
  constructor(connection) {
    this.connection = connection;
  }
  get account() {
    return new AccountTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.Account));
  }
  get accountCustomization() {
    return new AccountCustomizationTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.AccountCustomization));
  }
  get accountSeq() {
    return new AccountSeqTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.AccountSeq));
  }
  get admin() {
    return new AdminTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.Admin));
  }
  get authFrame() {
    return new AuthFrameTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.AuthFrame));
  }
  get baseCfg() {
    return new BaseCfgTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.BaseCfg));
  }
  get clock() {
    return new ClockTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.Clock));
  }
  get clockSchedule() {
    return new ClockScheduleTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.ClockSchedule));
  }
  get determinismCheck() {
    return new DeterminismCheckTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.DeterminismCheck));
  }
  get gameCoreSnap() {
    return new GameCoreSnapTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.GameCoreSnap));
  }
  get inputCollector() {
    return new InputCollectorTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.InputCollector));
  }
  get inputFrame() {
    return new InputFrameTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.InputFrame));
  }
  get lastAuthFrameTimestamp() {
    return new LastAuthFrameTimestampTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.LastAuthFrameTimestamp));
  }
  get levelFileData() {
    return new LevelFileDataTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.LevelFileData));
  }
  get seq() {
    return new SeqTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.Seq));
  }
  get stepsSinceLastAuthFrame() {
    return new StepsSinceLastAuthFrameTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.StepsSinceLastAuthFrame));
  }
  get stepsSinceLastBatch() {
    return new StepsSinceLastBatchTableHandle(this.connection.clientCache.getOrCreateTable(REMOTE_MODULE.tables.StepsSinceLastBatch));
  }
}
class SubscriptionBuilder extends SubscriptionBuilderImpl {
}
class DbConnection extends DbConnectionImpl {
  static builder = () => {
    return new DbConnectionBuilder(REMOTE_MODULE, (imp) => imp);
  };
  subscriptionBuilder = () => {
    return new SubscriptionBuilder(this);
  };
}
const PROFILE_STORAGE_PREFIX = "pfp/";
const MAX_PROFILE_VERSION = 255;
const PFP_MIME = "image/webp";
const PFP_MAX_BYTES = 512 * 1024;
const PFP_IMAGE_SIZE = 256;
function readUint32LE(data, offset) {
  return (data[offset] | data[offset + 1] << 8 | data[offset + 2] << 16 | data[offset + 3] << 24) >>> 0;
}
function getFourCC(data, offset) {
  return String.fromCharCode(data[offset], data[offset + 1], data[offset + 2], data[offset + 3]);
}
function parseLossyDimensions(data, offset, size) {
  if (size < 10) return null;
  const frameStart = offset;
  if (data[frameStart + 3] !== 157 || data[frameStart + 4] !== 1 || data[frameStart + 5] !== 42) {
    return null;
  }
  const rawWidth = data[frameStart + 6] | (data[frameStart + 7] & 63) << 8;
  const rawHeight = data[frameStart + 8] | (data[frameStart + 9] & 63) << 8;
  return { width: rawWidth + 1, height: rawHeight + 1 };
}
function parseLosslessDimensions(data, offset, size) {
  if (size < 5) return null;
  if (data[offset] !== 47) {
    return null;
  }
  const widthMinusOne = (data[offset + 2] & 63) << 8 | data[offset + 1];
  const heightMinusOne = (data[offset + 4] & 15) << 10 | data[offset + 3] << 2 | (data[offset + 2] & 192) >> 6;
  return { width: widthMinusOne + 1, height: heightMinusOne + 1 };
}
function parseExtendedDimensions(data, offset, size) {
  if (size < 10) return null;
  const widthMinusOne = data[offset + 4] | data[offset + 5] << 8 | data[offset + 6] << 16;
  const heightMinusOne = data[offset + 7] | data[offset + 8] << 8 | data[offset + 9] << 16;
  return { width: widthMinusOne + 1, height: heightMinusOne + 1 };
}
function parseWebpDimensions(data) {
  if (data.length < 30) {
    return null;
  }
  if (getFourCC(data, 0) !== "RIFF" || getFourCC(data, 8) !== "WEBP") {
    return null;
  }
  let offset = 12;
  while (offset + 8 <= data.length) {
    const chunkId = getFourCC(data, offset);
    const chunkSize = readUint32LE(data, offset + 4);
    const chunkDataOffset = offset + 8;
    if (chunkDataOffset + chunkSize > data.length) {
      return null;
    }
    if (chunkId === "VP8 ") {
      return parseLossyDimensions(data, chunkDataOffset, chunkSize);
    }
    if (chunkId === "VP8L") {
      return parseLosslessDimensions(data, chunkDataOffset, chunkSize);
    }
    if (chunkId === "VP8X") {
      return parseExtendedDimensions(data, chunkDataOffset, chunkSize);
    }
    offset = chunkDataOffset + chunkSize + (chunkSize & 1);
  }
  return null;
}
async function subscribeAndWait(connection, queries) {
  return new Promise((resolve, reject) => {
    let settled = false;
    connection.subscriptionBuilder().onApplied(() => {
      if (settled) return;
      settled = true;
      resolve();
    }).onError((errorContext) => {
      if (settled) return;
      settled = true;
      const errorMessage = errorContext instanceof Error ? errorContext.message : JSON.stringify(errorContext);
      console.error("[Profile] Subscription error:", errorMessage);
      reject(new Error(`Failed to subscribe: ${errorMessage}`));
    }).subscribe(queries);
  });
}
async function connectToSpacetimeDb(token) {
  const host = "ws://localhost:3000";
  const moduleName = "marbles2";
  return new Promise((resolve, reject) => {
    let settled = false;
    const builder = DbConnection.builder().withUri(host).withModuleName(moduleName).withToken(token).onConnect((connection, identity) => {
      if (identity === void 0) {
        if (!settled) {
          settled = true;
          reject(new Error("Unable to resolve account identity"));
        }
        return;
      }
      if (!settled) {
        settled = true;
        resolve({ connection, identity });
      }
    }).onConnectError((_ctx, err) => {
      if (!settled) {
        settled = true;
        reject(err ?? new Error("Failed to connect to SpacetimeDB"));
      }
    }).onDisconnect((_ctx, err) => {
      if (!settled) {
        settled = true;
        reject(err ?? new Error("Disconnected before connecting to SpacetimeDB"));
      }
    });
    builder.build();
  });
}
async function resolveAccountData(connection, identity) {
  console.log(`[Profile] Starting resolve account data for identity: ${identity.toHexString()}`);
  await subscribeAndWait(connection, [
    `SELECT * FROM Account`
    // WHERE identity = 0x${identity.toHexString()}`
  ]);
  console.log("[Profile] After subscribe and wait");
  const allAccounts = Array.from(connection.db.account.iter());
  console.log(`[Profile] Accounts in cache: ${allAccounts.length}`);
  for (const acc of allAccounts) {
    console.log(`[Profile]   Account ${acc.id}: identity=${acc.identity.toHexString()}`);
  }
  const accountRow = connection.db.account.identity.find(identity);
  if (!accountRow) {
    console.error(
      `[Profile] No account is associated with this identity: ${identity.toHexString()}`
    );
    throw error(469, "No account is associated with this identity");
  }
  const accountId = accountRow.id;
  const query = `SELECT * FROM AccountCustomization`;
  console.log("Starting subscribe for account_customization with Query: ", query);
  await subscribeAndWait(connection, [query]);
  console.log("After subscribe and wait for account_customization");
  const customization = connection.db.accountCustomization.accountId.find(accountId);
  const currentVersion = customization ? Number(customization.pfpVersion) : 0;
  return { accountId, currentVersion };
}
function buildR2TargetUrl(endpoint, bucket, objectKey) {
  const u = new URL(endpoint);
  const key = objectKey.replace(/^\/+/, "");
  const pathSegs = u.pathname.split("/").filter(Boolean);
  const hasBucketInPath = pathSegs.length > 0 && pathSegs[0] === bucket;
  const hasBucketInHost = u.hostname.startsWith(`${bucket}.`);
  if (hasBucketInPath) {
    u.pathname = `/${pathSegs.join("/")}/${key}`.replace(/\/{2,}/g, "/");
  } else if (hasBucketInHost) {
    u.pathname = `/${key}`;
  } else {
    u.pathname = `/${bucket}/${key}`;
  }
  return u.toString();
}
async function uploadProfilePictureToR2(body, objectKey, contentType, env) {
  const endpoint = env.PROFILE_PICTURE_S3_ENDPOINT;
  const bucket = env.PROFILE_PICTURE_S3_BUCKET;
  const accessKeyId = env.PROFILE_PICTURE_S3_ACCESS_KEY_ID;
  const secretAccessKey = env.PROFILE_PICTURE_S3_SECRET_ACCESS_KEY;
  if (!endpoint || !bucket || !accessKeyId || !secretAccessKey) {
    throw error(500, "Profile picture storage is not fully configured");
  }
  const awsClient = new AwsClient({
    accessKeyId,
    secretAccessKey,
    service: "s3",
    region: "auto"
  });
  const targetUrl = buildR2TargetUrl(endpoint, bucket, objectKey);
  console.log("[Profile] Uploading to R2:", targetUrl);
  const res = await awsClient.fetch(targetUrl, {
    method: "PUT",
    body,
    headers: {
      "Content-Type": contentType,
      "Cache-Control": "public, max-age=31536000, immutable"
    }
  });
  if (!res.ok) {
    const text = await res.text().catch(() => "");
    throw new Error(
      `Failed to upload profile picture to R2 (${res.status}): ${text || "No response body"}`
    );
  }
}
function buildProfilePictureUrl(accountId, version, env) {
  const baseUrl = env.PROFILE_PICTURE_BASE_URL?.trim();
  if (!baseUrl || version <= 0) {
    return null;
  }
  const normalizedBase = baseUrl.replace(/\/$/, "");
  return `${normalizedBase}/pfp/${accountId.toString()}.webp?v=${version}`;
}
async function downloadImageFromUrl(imageUrl) {
  const response = await fetch(imageUrl);
  if (!response.ok) {
    throw new Error(`Failed to download image from URL: ${response.status}`);
  }
  const buffer = await response.arrayBuffer();
  return new Uint8Array(buffer);
}
const POST = async ({ request, platform }) => {
  console.log("[Profile] Starting POST request");
  const env = platform?.env;
  if (!env) {
    throw error(500, "Platform environment is not available");
  }
  const authHeader = request.headers.get("authorization") ?? request.headers.get("Authorization");
  if (!authHeader?.startsWith("Bearer ")) {
    throw error(401, "An ID token is required to upload profile pictures");
  }
  const token = authHeader.slice(7).trim();
  if (!token) {
    throw error(401, "An ID token is required to upload profile pictures");
  }
  console.log(`[Profile] Received token (first 20 chars): ${token.substring(0, 20)}...`);
  const contentType = request.headers.get("content-type") ?? "";
  let bytes;
  if (contentType.includes("multipart/form-data")) {
    const formData = await request.formData();
    const image = formData.get("image");
    if (!(image instanceof File)) {
      throw error(400, "The request must include an image file");
    }
    if (image.type !== PFP_MIME) {
      throw error(400, `Profile pictures must be uploaded as ${PFP_MIME}`);
    }
    if (image.size > PFP_MAX_BYTES) {
      throw error(400, `Profile pictures must be smaller than ${PFP_MAX_BYTES} bytes`);
    }
    bytes = new Uint8Array(await image.arrayBuffer());
    const dimensions = parseWebpDimensions(bytes);
    if (!dimensions) {
      throw error(400, "The uploaded image is not a valid WebP file");
    }
    if (dimensions.width !== PFP_IMAGE_SIZE || dimensions.height !== PFP_IMAGE_SIZE) {
      throw error(
        400,
        `Profile pictures must be exactly ${PFP_IMAGE_SIZE}×${PFP_IMAGE_SIZE} pixels`
      );
    }
  } else if (contentType.includes("application/json")) {
    const body = await request.json();
    if (!body.imageUrl || typeof body.imageUrl !== "string") {
      throw error(400, "Request must include an imageUrl");
    }
    try {
      new URL(body.imageUrl);
    } catch {
      throw error(400, "Invalid imageUrl provided");
    }
    console.log("[Profile] Downloading image from URL:", body.imageUrl);
    try {
      bytes = await downloadImageFromUrl(body.imageUrl);
    } catch (err) {
      console.error("[Profile] Failed to download image:", err);
      throw error(400, "Failed to download image from URL");
    }
    console.log(`[Profile] Downloaded ${bytes.length} bytes from URL`);
  } else {
    throw error(400, "Request must be multipart/form-data or application/json");
  }
  let connection = null;
  let accountId = 0n;
  let currentVersion = 0;
  try {
    const connectionResult = await connectToSpacetimeDb(token);
    connection = connectionResult.connection;
    const accountData = await resolveAccountData(connection, connectionResult.identity);
    accountId = accountData.accountId;
    currentVersion = accountData.currentVersion;
    console.log(`[Profile] Resolved account ${accountId} with pfp version ${currentVersion}`);
  } catch (err) {
    console.error("[Profile] Failed to resolve account data:", err);
    connection?.disconnect();
    if (err && typeof err === "object" && "status" in err) {
      throw err;
    }
    throw error(401, "Unable to verify your SpacetimeDB account");
  }
  if (!connection) {
    throw error(500, "SpacetimeDB connection is not available");
  }
  const activeConnection = connection;
  const isWebp = bytes.length >= 12 && getFourCC(bytes, 8) === "WEBP";
  const extension = isWebp ? "webp" : "jpg";
  const mimeType = isWebp ? "image/webp" : "image/jpeg";
  const objectKey = `${PROFILE_STORAGE_PREFIX}${accountId.toString()}.${extension}`;
  const nextVersion = Math.min(currentVersion + 1, MAX_PROFILE_VERSION);
  try {
    console.log(`[Profile] Uploading ${bytes.length} bytes to R2 as ${objectKey}`);
    await uploadProfilePictureToR2(bytes.buffer, objectKey, mimeType, env);
    console.log("[Profile] Upload successful");
    console.log("[Profile] Calling IncrementPfpVersion reducer");
    await activeConnection.reducers.incrementPfpVersion();
    console.log("[Profile] Reducer called successfully");
  } catch (err) {
    console.error("[Profile] Failed to upload profile picture:", err);
    throw error(500, "Failed to upload profile picture");
  } finally {
    activeConnection.disconnect();
  }
  const url = buildProfilePictureUrl(accountId, nextVersion, env);
  console.log("[Profile] Returning profile picture URL:", url);
  return json({
    accountId: accountId.toString(),
    version: nextVersion,
    url
  });
};
export {
  POST
};
