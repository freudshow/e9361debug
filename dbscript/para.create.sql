--
-- SQLiteStudio v3.4.4 生成的文件，周一 9月 18 18:25:45 2023
--
-- 所用的文本编码：UTF-8
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- 表：t_afterCheck
DROP TABLE IF EXISTS t_afterCheck;

CREATE TABLE IF NOT EXISTS t_afterCheck (
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

INSERT INTO t_afterCheck (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 5, '{
	"IsUploadFileToTerminal": true,
	"FullFileNameComputer": "upload\\rc.local",
	"FullFileNameTerminal": "/etc/rc.local"
}', 6, '', 0, '下载系统启动脚本', 1, 3000, NULL);
INSERT INTO t_afterCheck (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 1, 'chmod +x /etc/rc.local', 6, '', 0, '给系统启动脚本赋予可执行权限', 1, 3000, NULL);
INSERT INTO t_afterCheck (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 1, ' /bin/mosquitto_pub   -t "e9361app/set/request/e9361esdkapp/version"  -m  "{\"token\": \"200513\", \"timestamp\": \"2023-02-11T09:41:09.845+0800\", \"iHardVer\": \"V1.01.03\", \"iSoftdVer\": \"V1.00.04\", \"eHardVer\": \"SV01.03\", \"eSoftdVer\": \"SV01.004\", \"runtime_min\": 6, \"upprogram\": 0, \"watchdog\": 1 }" -h localhost
', 6, '', 0, '停止看门狗', 1, 3000, NULL);
INSERT INTO t_afterCheck (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 1, 'ps | grep e9361esdkapp | awk ''{print  $1}'' | xargs kill -9', 6, '', 0, '杀死esdk进程', 1, 3000, NULL);
INSERT INTO t_afterCheck (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 5, '{
	"IsUploadFileToTerminal": true,
	"FullFileNameComputer": "upload\\e9361esdkapp",
	"FullFileNameTerminal": "/home/sysadm/src/e9361esdkapp"
}', 6, '', 0, '下载esdk', 1, 3000, NULL);
INSERT INTO t_afterCheck (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, 1, 'chmod +x /etc/rc.local', 6, '', 0, '给esdk赋予可执行权限', 1, 3000, NULL);
INSERT INTO t_afterCheck (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (7, 1, '/sbin/reboot', 6, '', 0, '重启终端', 1, 3000, NULL);
INSERT INTO t_afterCheck (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (8, 6, '', 6, '', 0, '等待终端重启... ...', 1, 20000, NULL);

-- 表：t_basePara
DROP TABLE IF EXISTS t_basePara;

CREATE TABLE IF NOT EXISTS t_basePara (
    seq   INTEGER   PRIMARY KEY AUTOINCREMENT,
    name  CHAR (50) UNIQUE,
    value CHAR (50) 
);

INSERT INTO t_basePara (seq, name, value) VALUES (1, 'Base_Para_IP_Address', '192.168.0.232');
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
INSERT INTO t_basePara (seq, name, value) VALUES (12, 'Base_Para_Upload_Directory', 'upload');
INSERT INTO t_basePara (seq, name, value) VALUES (13, 'Base_Para_Download_Directory', 'download');

-- 表：t_checkADE9078
DROP TABLE IF EXISTS t_checkADE9078;

CREATE TABLE IF NOT EXISTS t_checkADE9078 (
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

INSERT INTO t_checkADE9078 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 9, '{
	"RouteList": [
		{
			"RouteNo": 0,
			"ItemList": [
				{
					"RealDatabaseNo": 125,
					"ItemName": "A相电流",
					"StandardValue": 5.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 126,
					"ItemName": "B相电流",
					"StandardValue": 5.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 127,
					"ItemName": "C相电流",
					"StandardValue": 5.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 128,
					"ItemName": "A相电压",
					"StandardValue": 220.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 129,
					"ItemName": "B相电压",
					"StandardValue": 220.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 130,
					"ItemName": "C相电压",
					"StandardValue": 220.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 131,
					"ItemName": "A相有功功率",
					"StandardValue": 550.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 132,
					"ItemName": "B相有功功率",
					"StandardValue": 550.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 133,
					"ItemName": "C相有功功率",
					"StandardValue": 550.0,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 134,
					"ItemName": "A相无功功率",
					"StandardValue": 952.6279,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 135,
					"ItemName": "B相无功功率",
					"StandardValue": 952.6279,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				},
				{
					"RealDatabaseNo": 136,
					"ItemName": "C相无功功率",
					"StandardValue": 952.6279,
					"ErrorThresholdType": 2,
					"ErrorThreshold": 5.0
				}
			]
		}
	]
}', 2, 'True', 0, 'ADE9078整定', 1, 3000, NULL);

-- 表：t_checkConsolePort
DROP TABLE IF EXISTS t_checkConsolePort;

CREATE TABLE IF NOT EXISTS t_checkConsolePort (
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

INSERT INTO t_checkConsolePort (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 10, '
', 6, '\w*', 4, '敲击回车', 1, 1000, NULL);
INSERT INTO t_checkConsolePort (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 10, 'root
', 6, '\w*', 4, '输入用户名', 1, 1000, NULL);
INSERT INTO t_checkConsolePort (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 10, '123456
', 6, '\w*', 4, '输入密码', 1, 1000, NULL);
INSERT INTO t_checkConsolePort (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 10, 'ls /home/sysadm/src/
', 6, '\w*imx6ull\w*', 4, '读取e9361程序', 1, 1000, NULL);

-- 表：t_checkEncChip
DROP TABLE IF EXISTS t_checkEncChip;

CREATE TABLE IF NOT EXISTS t_checkEncChip (
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

INSERT INTO t_checkEncChip (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 3, '{
    "RealDataBaseNo": 29,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '加密芯片检测', 1, 3000, NULL);
INSERT INTO t_checkEncChip (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 3, '{
    "RealDataBaseNo": 28,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '4G/5G模块检测', 1, 3000, NULL);

-- 表：t_checkGPS
DROP TABLE IF EXISTS t_checkGPS;

CREATE TABLE IF NOT EXISTS t_checkGPS (
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

INSERT INTO t_checkGPS (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 3, '{
    "RealDataBaseNo": 322,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=120.0&&f<=123.0', 5, '读取经度', 1, 5000, NULL);
INSERT INTO t_checkGPS (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 3, '{
    "RealDataBaseNo": 323,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=36.0&&f<=39.0', 5, '读取纬度', 1, 5000, NULL);

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
    timeout        INTEGER DEFAULT (1),
    childTableName TEXT    UNIQUE
);

INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, NULL, NULL, NULL, NULL, NULL, '检测前的预备工作', 1, 1, 't_preCheckSteps');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, NULL, NULL, NULL, NULL, NULL, '遥控遥信检测', 1, 1, 't_checkYKYX');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, NULL, NULL, NULL, NULL, NULL, '检测网口状态', 1, 1, 't_checkNetPort');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, NULL, NULL, NULL, NULL, NULL, '检测Console口', 1, 1, 't_checkConsolePort');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, NULL, NULL, NULL, NULL, NULL, '加密芯片及4G/5G检测', 1, 1, 't_checkEncChip');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, NULL, NULL, NULL, NULL, NULL, 'Oled液晶检测', 1, 1, 't_checkManual');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (7, NULL, NULL, NULL, NULL, NULL, 'ADE9078检测', 1, 1, 't_checkADE9078');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (8, NULL, NULL, NULL, NULL, NULL, 'USB检测', 1, 1, 't_checkUSB');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (9, NULL, NULL, NULL, NULL, NULL, '按键检测', 1, 1, 't_checkKeyPress');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (10, NULL, NULL, NULL, NULL, NULL, 'GPS检测', 1, 1, 't_checkGPS');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (11, NULL, NULL, NULL, NULL, NULL, '端口检测', 1, 1, 't_checkPorts');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (12, NULL, NULL, NULL, NULL, NULL, 'PT100检测', 1, 1, 't_checkPT100');
INSERT INTO t_checkItemsBase (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (13, NULL, NULL, NULL, NULL, NULL, 'RS-232检测', 1, 1, 't_checkRS232');

-- 表：t_checkKeyPress
DROP TABLE IF EXISTS t_checkKeyPress;

CREATE TABLE IF NOT EXISTS t_checkKeyPress (
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

INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 8, '请按下"返回"键, 并保持5秒', 6, '', 0, '按下返回键', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 3, '{
    "RealDataBaseNo": 16,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '读取返回键的值', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 8, '请按下"左"键, 并保持5秒', 6, '', 0, '按下左键', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 3, '{
    "RealDataBaseNo": 13,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '读取左键的值', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 8, '请按下"下"键, 并保持5秒', 6, '', 0, '按下下键', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, 3, '{
    "RealDataBaseNo": 14,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '读取下键的值', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (7, 8, '请按下"复归"键, 并保持5秒', 6, '', 0, '按下复归键', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (8, 3, '{
    "RealDataBaseNo": 19,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '读取复归键的值', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (9, 8, '请按下"上"键, 并保持5秒', 6, '', 0, '按下上键', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (10, 3, '{
    "RealDataBaseNo": 17,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '读取上键的值', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (11, 8, '请按下"右"键, 并保持5秒', 6, '', 0, '按下右键', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (12, 3, '{
    "RealDataBaseNo": 15,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '读取右键的值', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (13, 8, '请按下"确认"键, 并保持5秒', 6, '', 0, '按下确认键', 1, 2000, NULL);
INSERT INTO t_checkKeyPress (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (14, 3, '{
    "RealDataBaseNo": 18,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 1, '1.0', 0, '读取确认键的值', 1, 2000, NULL);

-- 表：t_checkManual
DROP TABLE IF EXISTS t_checkManual;

CREATE TABLE IF NOT EXISTS t_checkManual (
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

INSERT INTO t_checkManual (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 8, 'Oled液晶是否正常显示?', 2, 'True', 0, 'Oled液晶检测', 1, 3000, NULL);

-- 表：t_checkNetPort
DROP TABLE IF EXISTS t_checkNetPort;

CREATE TABLE IF NOT EXISTS t_checkNetPort (
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

INSERT INTO t_checkNetPort (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 7, 'ping 192.168.0.232', 6, '时间<\dms TTL=\d', 4, '网口1测试', 1, 5000, NULL);
INSERT INTO t_checkNetPort (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 7, 'ping 192.168.1.232', 6, '时间<\dms TTL=\d', 4, '网口2测试', 1, 5000, NULL);
INSERT INTO t_checkNetPort (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 7, 'ping 192.168.2.232', 6, '时间<\dms TTL=\d', 4, '网口3测试', 1, 5000, NULL);
INSERT INTO t_checkNetPort (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 7, 'ping 192.168.3.232', 6, '时间<\dms TTL=\d', 4, '网口4测试', 1, 5000, NULL);

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
}', 1, '(f)=>f>=50.0&&f<=300.0', 5, 'RS485-1测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 3, '{
    "RealDataBaseNo": 318,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=50.0&&f<=300.0', 5, 'RS485-2测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 3, '{
    "RealDataBaseNo": 319,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=50.0&&f<=300.0', 5, 'RS485-3测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 3, '{
    "RealDataBaseNo": 320,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=50.0&&f<=300.0', 5, 'CAN测试', 1, 5000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 6, '', 6, '', 0, '等待CCO组网... ...', 1, 3000, NULL);
INSERT INTO t_checkPorts (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, 3, '{
    "RealDataBaseNo": 321,
    "TeleType": 1,
    "DataType": 0,
    "DataItemCount": 1
}', 1, '(f)=>f>=50.0&&f<=300.0', 5, 'CCO测试', 1, 5000, NULL);

-- 表：t_checkPT100
DROP TABLE IF EXISTS t_checkPT100;

CREATE TABLE IF NOT EXISTS t_checkPT100 (
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

INSERT INTO t_checkPT100 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 12, '{
	"RealDatabaseNo": 54,
	"ItemName": "当前温度(℃)",
	"StandardValue": 0.0,
	"ErrorThresholdType": 0,
	"ErrorThreshold": 3.0
}', 1, '0.0', 2, 'PT100整定', 1, 5000, NULL);

-- 表：t_checkRS232
DROP TABLE IF EXISTS t_checkRS232;

CREATE TABLE IF NOT EXISTS t_checkRS232 (
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

INSERT INTO t_checkRS232 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 5, '{
	"IsUploadFileToTerminal": true,
	"FullFileNameComputer": "upload\\serial",
	"FullFileNameTerminal": "/bin/serial"
}', 6, '', 0, '下载串口测试程序', 1, 3000, NULL);
INSERT INTO t_checkRS232 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 1, 'chmod +x /bin/serial', 6, '', 0, '给串口测试程序赋予可执行权限', 1, 3000, NULL);
INSERT INTO t_checkRS232 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 1, ' /bin/mosquitto_pub   -t "e9361app/set/request/e9361esdkapp/version"  -m  "{\"token\": \"200513\", \"timestamp\": \"2023-02-11T09:41:09.845+0800\", \"iHardVer\": \"V1.01.03\", \"iSoftdVer\": \"V1.00.04\", \"eHardVer\": \"SV01.03\", \"eSoftdVer\": \"SV01.004\", \"runtime_min\": 6, \"upprogram\": 0, \"watchdog\": 1 }" -h localhost
', 6, '', 0, '停止看门狗', 1, 3000, NULL);
INSERT INTO t_checkRS232 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 1, 'ps | grep e9361app | awk ''{print  $1}'' | xargs kill -9', 6, '', 0, '杀死e9361app进程', 1, 3000, NULL);
INSERT INTO t_checkRS232 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 1, '/bin/serial -c "/dev/ttymxc2" -b 9600 -p 0 -f"68 18 09 15 04 22 20 68 11 04 33 34 34 35 31 16" -t 1', 6, 'FE FE FE FE 68 18 09 15 04 22 20 68 91 06 33 34 34 35 ([0-9a-fA-F]{2}\s){3}16', 4, '读取A相电压', 1, 3000, NULL);
INSERT INTO t_checkRS232 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, 1, '/sbin/reboot', 6, '', 0, '重启终端', 1, 3000, NULL);
INSERT INTO t_checkRS232 (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (7, 6, '', 6, '', 0, '等待终端重启... ...', 1, 20000, NULL);

-- 表：t_checkUSB
DROP TABLE IF EXISTS t_checkUSB;

CREATE TABLE IF NOT EXISTS t_checkUSB (
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

INSERT INTO t_checkUSB (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 8, '请将U盘插入E9361-C0的USB口', 6, '', 0, '插入U盘', 1, 3000, NULL);
INSERT INTO t_checkUSB (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 1, 'ls /dev/sd*', 6, '\w*/dev/sda\w*', 4, 'U盘检测', 1, 2000, NULL);
INSERT INTO t_checkUSB (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 8, '请将U盘移除', 6, '', 0, '移除U盘', 1, 1000, NULL);

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
}', 2, '0', 0, '使能预置', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 4, '{
    "YKOperateType": 2,
    "YKNo": 1,
    "YKOperation": 81,
    "DelayTime": 2000
}', 2, '0', 0, '遥控1合', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 3, '{
    "RealDataBaseNo": 9,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '1', 0, '读取遥信1的值', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 3, '{
    "RealDataBaseNo": 10,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '1', 0, '读取遥信2的值', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 4, '{
    "YKOperateType": 2,
    "YKNo": 1,
    "YKOperation": 145,
    "DelayTime": 2000
}', 2, '0', 0, '遥控1分', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, 3, '{
    "RealDataBaseNo": 9,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '0', 0, '读取遥信1的值', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (7, 3, '{
    "RealDataBaseNo": 10,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '0', 0, '读取遥信2的值', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (8, 4, '{
    "YKOperateType": 2,
    "YKNo": 2,
    "YKOperation": 81,
    "DelayTime": 2000
}', 2, '0', 0, '遥控2合', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (9, 3, '{
    "RealDataBaseNo": 11,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '1', 0, '读取遥信3的值', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (10, 3, '{
    "RealDataBaseNo": 12,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '1', 0, '读取遥信4的值', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (11, 4, '{
    "YKOperateType": 2,
    "YKNo": 2,
    "YKOperation": 145,
    "DelayTime": 2000
}', 2, '0', 0, '遥控2分', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (12, 3, '{
    "RealDataBaseNo": 11,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '0', 0, '读取遥信3的值', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (13, 3, '{
    "RealDataBaseNo": 12,
    "TeleType": 0,
    "DataType": 1,
    "DataItemCount": 1
}', 0, '0', 0, '读取遥信4的值', 1, 5000, NULL);
INSERT INTO t_checkYKYX (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (14, 4, '{
    "YKOperateType": 3,
    "YKNo": 0,
    "YKOperation": 145,
    "DelayTime": 2000
}', 2, '0', 0, '撤销预置', 1, 5000, NULL);

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
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (2, 0, 'Cmd_Type_MaintainFrame', '使用维护规约发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (3, 1, 'Cmd_Type_Shell', '使用shell命令发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (4, 2, 'Cmd_Type_Mqtt', '使用mqtt发送命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (5, 3, 'Cmd_Type_MaintainReadRealDataBase', '读取一个实时库数值');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (6, 4, 'Cmd_Type_MaintainWriteRealDataBaseYK', '控制一个遥控的开/合');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (7, 5, 'Cmd_Type_SftpFileTransfer', '用Sftp协议传输文件');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (8, 6, 'Cmd_Type_DelaySomeTime', '单纯的延时操作');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (9, 7, 'Cmd_Type_WindowsCommand', '执行Windows命令');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (10, 8, 'Cmd_Type_Manual_Operate', '手动让用户操作');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (11, 9, 'Cmd_Type_ADC_Adjust', '交采整定');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (12, 10, 'Cmd_Type_Console', '维护口检测');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (13, 11, 'Cmd_Type_SetSerial', '设置终端序列号或者ESN号等');
INSERT INTO t_cmdTypeEnum (seq, enum, enumName, discription) VALUES (14, 12, 'Cmd_Type_SetPT100', 'PT100整定');

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
INSERT INTO t_mqttTopics (seq, topics) VALUES (2, 'e9361esdkapp/set/response/e9361appversion');
INSERT INTO t_mqttTopics (seq, topics) VALUES (3, 'e9361esdkapp/set/response/e9361app/version');

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

INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (1, 1, ' /bin/mosquitto_pub   -t "e9361app/set/request/e9361esdkapp/version"  -m  "{\"token\": \"200513\", \"timestamp\": \"2023-02-11T09:41:09.845+0800\", \"iHardVer\": \"V1.01.03\", \"iSoftdVer\": \"V1.00.04\", \"eHardVer\": \"SV01.03\", \"eSoftdVer\": \"SV01.004\", \"runtime_min\": 6, \"upprogram\": 0, \"watchdog\": 1 }" -h localhost
', 6, '', 0, '停止看门狗', 1, 3000, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (2, 1, 'ps | grep e9361app | awk ''{print  $1}'' | xargs kill -9', 6, '', 0, '杀死e9361app进程', 1, 3000, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (3, 5, '{
	"IsUploadFileToTerminal": true,
	"FullFileNameComputer": "upload\\e9361app",
	"FullFileNameTerminal": "/home/sysadm/src/e9361app"
}', 6, '', 0, '下载最新的e9361app程序', 1, 3000, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (4, 1, 'chmod +x /home/sysadm/src/e9361app', 6, '', 0, '给e9361app赋予可执行权限', 1, 3000, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (5, 1, 'mkdir -p /data/app/e9361app/root/para && rm -rf /data/app/e9361app/root/para/*', 6, '', 0, '新建配置目录', 1, 3000, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (6, 5, '{
	"IsUploadFileToTerminal": true,
	"FullFileNameComputer": "upload\\c0_test.zip",
	"FullFileNameTerminal": "/data/app/e9361app/root/para/c0_test.zip"
}', 6, '', 0, '下载测试工装配置', 1, 5000, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (7, 1, 'unzip -x /data/app/e9361app/root/para/c0_test.zip -d /data/app/e9361app/root/para/ && mv /data/app/e9361app/root/para/localpara/localpara.json /data/app/e9361app/ && rm -rf /data/app/e9361app/root/para/c0_test.zip', 6, '\w*inflating\w*', 4, '解压配置文件并清理压缩包', 1, 3000, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (8, 1, '/sbin/reboot', 6, '', 0, '重启终端', 1, 3000, NULL);
INSERT INTO t_preCheckSteps (seq, cmdType, cmdParam, resultType, resultValue, resultSign, description, isEnable, timeout, childTableName) VALUES (9, 6, '', 6, '', 0, '等待终端重启... ...', 1, 60000, NULL);

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

INSERT INTO t_runtimeVariable (seq, name, value) VALUES (2, 'Console_Port_Baud', '115200');
INSERT INTO t_runtimeVariable (seq, name, value) VALUES (3, 'Console_Port_name', 'COM6');

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
