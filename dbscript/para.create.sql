--
-- SQLiteStudio v3.4.3 生成的文件，周日 5月 7 20:00:09 2023
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
    resultSign     INTEGER REFERENCES t_resultSignEnum (enum),
    description    TEXT    DEFAULT 测试项,
    isEnable       INTEGER REFERENCES t_isEnable (isEnable),
    childTableName TEXT
);

INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, childTableName) VALUES (1, NULL, NULL, NULL, NULL, NULL, '端口检测', 1, 't_checkPorts');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, childTableName) VALUES (2, NULL, NULL, NULL, NULL, NULL, '遥控遥信检测', 1, 't_checkYKYX');

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

INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 3, '{
    "RealDataBaseNo": 317,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=190.0&&f<=300.0', 5, 'RS485-1测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 3, '{
    "RealDataBaseNo": 318,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=190.0&&f<=300.0', 5, 'RS485-2测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 3, '{
    "RealDataBaseNo": 319,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=190.0&&f<=300.0', 5, 'RS485-3测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 3, '{
    "RealDataBaseNo": 320,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=190.0&&f<=300.0', 5, 'CAN测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 3, '{
    "RealDataBaseNo": 321,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=190.0&&f<=300.0', 5, 'CCO测试', 1, 5000, NULL);

-- 表：t_checkYKYX
DROP TABLE IF EXISTS t_checkYKYX;

CREATE TABLE IF NOT EXISTS t_checkYKYX (
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

INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 4, '{
    "YKOperateType": 1,
    "YKNo": 0,
    "YKOperation": 81,
    "DelayTime": 2000
}', 2, '0', 0, '使能预置', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 4, '{
    "YKOperateType": 2,
    "YKNo": 1,
    "YKOperation": 81,
    "DelayTime": 2000
}', 2, '0', 0, '遥控1合', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 3, '{
    "RealDataBaseNo": 9,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '1', 0, '读取遥信1的值', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 3, '{
    "RealDataBaseNo": 10,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '1', 0, '读取遥信2的值', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 4, '{
    "YKOperateType": 2,
    "YKNo": 1,
    "YKOperation": 145,
    "DelayTime": 2000
}', 2, '0', 0, '遥控1分', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, 3, '{
    "RealDataBaseNo": 9,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '0', 0, '读取遥信1的值', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (7, 3, '{
    "RealDataBaseNo": 10,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '0', 0, '读取遥信2的值', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (8, 4, '{
    "YKOperateType": 2,
    "YKNo": 2,
    "YKOperation": 81,
    "DelayTime": 2000
}', 2, '0', 0, '遥控2合', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (9, 3, '{
    "RealDataBaseNo": 11,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '1', 0, '读取遥信3的值', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (10, 3, '{
    "RealDataBaseNo": 12,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '1', 0, '读取遥信4的值', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (11, 4, '{
    "YKOperateType": 2,
    "YKNo": 2,
    "YKOperation": 145,
    "DelayTime": 2000
}', 2, '0', 0, '遥控2分', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (12, 3, '{
    "RealDataBaseNo": 11,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '0', 0, '读取遥信3的值', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (13, 3, '{
    "RealDataBaseNo": 12,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '0', 0, '读取遥信4的值', 1, 5, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (14, 4, '{
    "YKOperateType": 3,
    "YKNo": 0,
    "YKOperation": 145,
    "DelayTime": 2000
}', 2, '0', 0, '撤销预置', 1, 5, NULL);

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

INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (1, -1, 'Cmd_Type_Invalid', '不需要发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (2, 0, 'Cmd_MaintainFrame', '使用维护规约发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (3, 1, 'Cmd_Shell', '使用shell命令发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (4, 2, 'Cmd_Mqtt', '使用mqtt发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (5, 3, 'Cmd_MaintainReadRealDataBase', '读取一个实时库数值');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (6, 4, 'Cmd_MaintainWriteRealDataBaseYK', '控制一个遥控的开/合');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (7, 5, 'Cmd_SftpFileTransfer', '用Sftp协议传输文件');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (8, 6, 'Cmd_DelaySomeTime', '单纯的延时操作');

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

-- 表：t_mqttTopics
DROP TABLE IF EXISTS t_mqttTopics;

CREATE TABLE IF NOT EXISTS t_mqttTopics (
    seq    INTEGER PRIMARY KEY AUTOINCREMENT,
    topics TEXT    UNIQUE
);

INSERT INTO t_mqttTopics (seq, topics) VALUES (1, 'e9361app/set/request/e9361esdkapp/version');

-- 表：t_portBaudrateEnum
DROP TABLE IF EXISTS t_portBaudrateEnum;

CREATE TABLE IF NOT EXISTS t_portBaudrateEnum (
    seq      INTEGER PRIMARY KEY AUTOINCREMENT,
    enum     INTEGER,
    enumName TEXT
);

INSERT INTO t_portBaudrateEnum (seq, enum, enumName) VALUES (1, 1200, 'Baudrate_1200');
INSERT INTO t_portBaudrateEnum (seq, enum, enumName) VALUES (2, 2400, 'Baudrate_2400');
INSERT INTO t_portBaudrateEnum (seq, enum, enumName) VALUES (3, 4800, 'Baudrate_4800');
INSERT INTO t_portBaudrateEnum (seq, enum, enumName) VALUES (4, 9600, 'Baudrate_9600');
INSERT INTO t_portBaudrateEnum (seq, enum, enumName) VALUES (6, 19200, 'Baudrate_19200');
INSERT INTO t_portBaudrateEnum (seq, enum, enumName) VALUES (7, 38400, 'Baudrate_38400');
INSERT INTO t_portBaudrateEnum (seq, enum, enumName) VALUES (8, 57600, 'Baudrate_57600');
INSERT INTO t_portBaudrateEnum (seq, enum, enumName) VALUES (9, 115200, 'Baudrate_115200');

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

-- 表：t_preCheckSteps
DROP TABLE IF EXISTS t_preCheckSteps;

CREATE TABLE IF NOT EXISTS t_preCheckSteps (
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

INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 1, 'echo "allow_anonymous true">/etc/mosquitto.conf&&
echo "listener 1883">>/etc/mosquitto.conf&&
echo "persistence true">>/etc/mosquitto.conf&&
echo "persistence_location /mosquitto/data/">>/etc/mosquitto.conf&&
echo "log_dest file /mosquitto/log/mosquitto.log">>/etc/mosquitto.conf
', 6, '', 0, '修改mosquito配置文件', 1, 3, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 1, 'sed -i.bak -r ''s#/bin/mosquitto&#/bin/mosquitto -v -c /etc/mosquitto.conf&#g'' /etc/rc.local', 6, '', 0, '修改启动脚本rc.local', 1, 3, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 1, 'reboot', 6, '', 0, '重启终端', 1, 3, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 6, '', 6, '', 0, '等待终端重启... ...', 1, 20, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 1, 'ps | grep e9361app | awk ''{print  $1}'' | xargs kill -9', 6, '', 0, '杀死e9361app进程', 1, 3, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, 2, '{
	"Topic": "e9361app/set/request/e9361esdkapp/version",
	"Message": "{\"token\": \"200513\",\"timestamp\": \"2019-05-29T19:05:51.641+0800\",\"iHardVer\": \"V1.00.01\",\"iSoftdVer\": \"V1.00.07\",\"eHardVer\": \"SV01.01\",\"eSoftdVer\": \"SV01.007\",\"runtime_min\": 4,\"upprogram\": 0,\"watchdog\": 1}"
}', 6, '{
	"Topic": "e9361esdkapp/set/response/e9361appversion",
	"Message": "{ \"token\": \"200513\", \"timestamp\": \"2019-05-30T05:00:52.832+0800\" }"
}', 0, '停止看门狗', 1, 3, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (7, 5, '{
	"IsUploadFileToTerminal": true,
	"FullFileNameComputer": "upload\\e9361app",
	"FullFileNameTerminal": "/home/sysadm/src/e9361app"
}', 6, '', 0, '下载最新的e9361app程序', 1, 20, NULL);

-- 表：t_realDataTypeEnum
DROP TABLE IF EXISTS t_realDataTypeEnum;

CREATE TABLE IF NOT EXISTS t_realDataTypeEnum (
    seq         INTEGER PRIMARY KEY AUTOINCREMENT,
    enum        INTEGER NOT NULL
                        UNIQUE,
    enumName    TEXT    UNIQUE,
    description TEXT
);

INSERT INTO t_realDataTypeEnum (seq, enum, enumName, description) VALUES (1, -1, 'Real_Data_type_Invalid', '无效');
INSERT INTO t_realDataTypeEnum (seq, enum, enumName, description) VALUES (2, 0, 'Real_Data_type_Float', '浮点');
INSERT INTO t_realDataTypeEnum (seq, enum, enumName, description) VALUES (3, 1, 'Real_Data_type_Char', '字节');
INSERT INTO t_realDataTypeEnum (seq, enum, enumName, description) VALUES (4, 2, 'Real_Data_type_Int', '整数');

-- 表：t_realTeleTypeEnum
DROP TABLE IF EXISTS t_realTeleTypeEnum;

CREATE TABLE IF NOT EXISTS t_realTeleTypeEnum (
    seq         INTEGER PRIMARY KEY AUTOINCREMENT,
    enum        INTEGER NOT NULL
                        UNIQUE,
    enumName    TEXT    UNIQUE,
    description TEXT
);

INSERT INTO t_realTeleTypeEnum (seq, enum, enumName, description) VALUES (1, -1, 'Real_Data_TeleType_Invalid', '无效');
INSERT INTO t_realTeleTypeEnum (seq, enum, enumName, description) VALUES (2, 0, 'Real_Data_TeleType_YX', '遥信');
INSERT INTO t_realTeleTypeEnum (seq, enum, enumName, description) VALUES (3, 1, 'Real_Data_TeleType_YC', '遥测');
INSERT INTO t_realTeleTypeEnum (seq, enum, enumName, description) VALUES (4, 2, 'Real_Data_TeleType_YK', '遥控');
INSERT INTO t_realTeleTypeEnum (seq, enum, enumName, description) VALUES (5, 3, 'Real_Data_TeleType_DD', '电度');
INSERT INTO t_realTeleTypeEnum (seq, enum, enumName, description) VALUES (6, 4, 'Real_Data_TeleType_Parameter', '参数');

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

INSERT INTO t_runtimeVariable (seq, name, value) VALUES (2, 'Console_Port_Baud', '1200');
INSERT INTO t_runtimeVariable (seq, name, value) VALUES (3, 'Console_Port_name', 'COM1');

-- 表：t_YKOnOffEnum
DROP TABLE IF EXISTS t_YKOnOffEnum;

CREATE TABLE IF NOT EXISTS t_YKOnOffEnum (
    seq         INTEGER PRIMARY KEY AUTOINCREMENT,
    enum        INTEGER NOT NULL
                        UNIQUE,
    enumName    TEXT    UNIQUE,
    description TEXT
);

INSERT INTO t_YKOnOffEnum (seq, enum, enumName, description) VALUES (1, 81, 'YK_On', '遥控合');
INSERT INTO t_YKOnOffEnum (seq, enum, enumName, description) VALUES (2, 145, 'YK_Off', '遥控分');

-- 表：t_YKOperateTypeEnum
DROP TABLE IF EXISTS t_YKOperateTypeEnum;

CREATE TABLE IF NOT EXISTS t_YKOperateTypeEnum (
    seq         INTEGER PRIMARY KEY AUTOINCREMENT,
    enum        INTEGER NOT NULL
                        UNIQUE,
    enumName    TEXT    UNIQUE,
    description TEXT
);

INSERT INTO t_YKOperateTypeEnum (seq, enum, enumName, description) VALUES (1, 1, 'YK_Operate_Preset', '遥控预置合');
INSERT INTO t_YKOperateTypeEnum (seq, enum, enumName, description) VALUES (2, 2, 'YK_Operate_Actual', '遥控操作');
INSERT INTO t_YKOperateTypeEnum (seq, enum, enumName, description) VALUES (3, 3, 'YK_Operate_Cancel_Preset', '遥控预置撤销');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
