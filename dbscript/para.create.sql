--
-- SQLiteStudio v3.4.4 生成的文件，周五 4月 28 08:29:57 2023
--
-- 所用的文本编码：UTF-8
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- 表：t_basePara
DROP TABLE IF EXISTS t_basePara;

CREATE TABLE IF NOT EXISTS t_basePara (
    seq   INTEGER   PRIMARY KEY AUTOINCREMENT,
    name  CHAR (50) UNIQUE,
    value CHAR (50) 
);

INSERT INTO t_basePara (seq, name, value) VALUES (1, 'Base_Para_IP_Address', '192.168.0.237');
INSERT INTO t_basePara (seq, name, value) VALUES (2, 'Base_Para_SSH_Port', '22');
INSERT INTO t_basePara (seq, name, value) VALUES (3, 'Base_Para_User', 'root');
INSERT INTO t_basePara (seq, name, value) VALUES (4, 'Base_Para_Passwd', '123456');
INSERT INTO t_basePara (seq, name, value) VALUES (5, 'Base_Para_Mqtt_Publish_Topic', 'e9361debug/action/e9361debugapp/debug');
INSERT INTO t_basePara (seq, name, value) VALUES (6, 'Base_Para_Mqtt_Response_Topic', 'e9361debugapp/action/e9361debug/debug');
INSERT INTO t_basePara (seq, name, value) VALUES (7, 'Base_Para_Mqtt_Port', '1883');
INSERT INTO t_basePara (seq, name, value) VALUES (8, 'Base_Para_Maintain_Default_UDP_Port', '5000');
INSERT INTO t_basePara (seq, name, value) VALUES (9, 'Base_Para_Main_Check_Table', 't_checkItemsBase');
INSERT INTO t_basePara (seq, name, value) VALUES (10, 'Base_Para_Maintain_Port_Type', 'PortType_Net_UDP_Client');
INSERT INTO t_basePara (seq, name, value) VALUES (11, 'Base_Para_Maintain_Default_TCP_Client_Port', '5001');

-- 表：t_checkItemsBase
DROP TABLE IF EXISTS t_checkItemsBase;

CREATE TABLE IF NOT EXISTS t_checkItemsBase (
    seq            INTEGER PRIMARY KEY AUTOINCREMENT,
    cmdType        INTEGER REFERENCES t_cmdTypeEnum (enum),
    cmdParam       TEXT,
    resultType     INTEGER REFERENCES t_resultDataTypeEnum (enum),
    resultValue    TEXT,
    resultSign     INTEGER REFERENCES t_resultExpectEnum (seq),
    description    TEXT    DEFAULT 测试项,
    isEnable       INTEGER REFERENCES t_isEnable (isEnable),
    childTableName TEXT
);

INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, childTableName) VALUES (1, NULL, NULL, NULL, NULL, NULL, '端口检测', 1, 't_checkPorts');

-- 表：t_checkPorts
DROP TABLE IF EXISTS t_checkPorts;

CREATE TABLE IF NOT EXISTS t_checkPorts (
    seq            INTEGER PRIMARY KEY AUTOINCREMENT,
    cmdType        INTEGER NOT NULL
                           REFERENCES t_cmdTypeEnum (enum),
    cmdParam       TEXT    NOT NULL,
    resultType     INTEGER REFERENCES t_resultDataTypeEnum (enum) 
                           NOT NULL,
    resultValue    TEXT    NOT NULL,
    resultSign     INTEGER NOT NULL
                           REFERENCES t_resultSignEnum (enum),
    description    TEXT    DEFAULT 测试项
                           NOT NULL,
    isEnable       INTEGER REFERENCES t_isEnable (isEnable) 
                           NOT NULL,
    timeout        INTEGER,
    childTableName TEXT
);

INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 3, '317,1,0,1', 1, '(f)=>f>=210.0&&f<=230.0', 5, 'RS485-1测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 3, '318,1,0,1', 1, '(f)=>f>=210.0&&f<=230.0', 5, 'RS485-2测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 3, '319,1,0,1', 1, '(f)=>f>=210.0&&f<=230.0', 5, 'RS485-3测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 3, '320,1,0,1', 1, '(f)=>f>=210.0&&f<=230.0', 5, 'CAN测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 3, '321,1,0,1', 1, '(f)=>f>=210.0&&f<=230.0', 5, 'CCO测试', 1, 5000, NULL);

-- 表：t_cmdTypeEnum
DROP TABLE IF EXISTS t_cmdTypeEnum;

CREATE TABLE IF NOT EXISTS t_cmdTypeEnum (
    seq         INTEGER   PRIMARY KEY AUTOINCREMENT,
    enum        INTEGER   UNIQUE
                          NOT NULL,
    enumName    TEXT (50) UNIQUE
                          NOT NULL,
    discription TEXT
);

INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (1, 0, 'Cmd_MaintainFrame', '使用维护规约发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (2, 1, 'Cmd_Shell', '使用shell命令发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (3, 2, 'Cmd_Mqtt', '使用mqtt发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (4, 3, 'Cmd_MaintainReadRealDataBase', '读取一个实时库数值');

-- 表：t_isEnable
DROP TABLE IF EXISTS t_isEnable;

CREATE TABLE IF NOT EXISTS t_isEnable (
    seq         INTEGER PRIMARY KEY AUTOINCREMENT,
    isEnable    INTEGER NOT NULL
                        UNIQUE,
    description TEXT
);

INSERT INTO t_isEnable (seq, isEnable, description) VALUES (1, 1, '使能');
INSERT INTO t_isEnable (seq, isEnable, description) VALUES (2, 0, '不使能');

-- 表：t_PortTypeEnum
DROP TABLE IF EXISTS t_PortTypeEnum;

CREATE TABLE IF NOT EXISTS t_PortTypeEnum (
    seq      INTEGER PRIMARY KEY AUTOINCREMENT,
    enum     INTEGER UNIQUE,
    enumName TEXT    UNIQUE
);

INSERT INTO t_PortTypeEnum (seq, enum, enumName) VALUES (1, 0, 'PortType_Serial');
INSERT INTO t_PortTypeEnum (seq, enum, enumName) VALUES (2, 1, 'PortType_Net_UDP_Client');
INSERT INTO t_PortTypeEnum (seq, enum, enumName) VALUES (3, 2, 'PortType_Net_TCP_Client');
INSERT INTO t_PortTypeEnum (seq, enum, enumName) VALUES (4, 3, 'PortType_Net_TCP_Server');
INSERT INTO t_PortTypeEnum (seq, enum, enumName) VALUES (5, -1, 'PortType_Error');

-- 表：t_resultDataTypeEnum
DROP TABLE IF EXISTS t_resultDataTypeEnum;

CREATE TABLE IF NOT EXISTS t_resultDataTypeEnum (
    seq      INTEGER PRIMARY KEY AUTOINCREMENT,
    enum     INTEGER UNIQUE
                     NOT NULL,
    enumName TEXT    UNIQUE
                     NOT NULL
);

INSERT INTO t_resultDataTypeEnum (seq, enum, enumName) VALUES (1, 0, 'Result_Type_Int32');
INSERT INTO t_resultDataTypeEnum (seq, enum, enumName) VALUES (2, 1, 'Result_Type_Double');
INSERT INTO t_resultDataTypeEnum (seq, enum, enumName) VALUES (3, 2, 'Result_Type_Boolean');
INSERT INTO t_resultDataTypeEnum (seq, enum, enumName) VALUES (4, 3, 'Result_Type_Positive_Infinity');
INSERT INTO t_resultDataTypeEnum (seq, enum, enumName) VALUES (5, 4, 'Result_Type_Negtive_Infinity');
INSERT INTO t_resultDataTypeEnum (seq, enum, enumName) VALUES (6, 5, 'Result_Type_Byte_Array');
INSERT INTO t_resultDataTypeEnum (seq, enum, enumName) VALUES (7, 6, 'Result_Type_String');

-- 表：t_resultSignEnum
DROP TABLE IF EXISTS t_resultSignEnum;

CREATE TABLE IF NOT EXISTS t_resultSignEnum (
    seq      INTEGER PRIMARY KEY AUTOINCREMENT,
    enum     INTEGER UNIQUE
                     NOT NULL,
    enumName TEXT    UNIQUE
                     NOT NULL
);

INSERT INTO t_resultSignEnum (seq, enum, enumName) VALUES (1, 0, 'Result_Sign_Equal');
INSERT INTO t_resultSignEnum (seq, enum, enumName) VALUES (2, 1, 'Result_Sign_Greater_Than');
INSERT INTO t_resultSignEnum (seq, enum, enumName) VALUES (3, 2, 'Result_Sign_Less_Than');
INSERT INTO t_resultSignEnum (seq, enum, enumName) VALUES (4, 3, 'Result_Sign_Interval');
INSERT INTO t_resultSignEnum (seq, enum, enumName) VALUES (5, 4, 'Result_Sign_Regex');
INSERT INTO t_resultSignEnum (seq, enum, enumName) VALUES (6, 5, 'Result_Sign_Lambda');

-- 表：t_runtimeVariable
DROP TABLE IF EXISTS t_runtimeVariable;

CREATE TABLE IF NOT EXISTS t_runtimeVariable (
    seq   INTEGER PRIMARY KEY AUTOINCREMENT,
    name  TEXT    UNIQUE
                  NOT NULL,
    value TEXT    NOT NULL
);

INSERT INTO t_runtimeVariable (seq, name, value) VALUES (1, 'Console_Port_name', 'COM3');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
