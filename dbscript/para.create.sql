--
-- 由SQLiteStudio v3.2.1 产生的文件 周三 8月 14 17:00:35 2019
--
-- 文本编码：System
--
PRAGMA encoding = "UTF-8"; 
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- 表：t_basepara
DROP TABLE IF EXISTS t_basepara;
CREATE TABLE t_basepara (
    id    INTEGER   PRIMARY KEY AUTOINCREMENT,
    name  CHAR (50),
    value CHAR (50)
);
INSERT INTO t_basepara (name, value) VALUES ('host', '192.168.0.10');
INSERT INTO t_basepara (name, value) VALUES ('usr', 'root');
INSERT INTO t_basepara (name, value) VALUES ('pwd', '1');
INSERT INTO t_basepara (name, value) VALUES ('idPrefix', '0001007');
INSERT INTO t_basepara (name, value) VALUES ('idSuffixLen', '5');

-- 表：t_chkPara
DROP TABLE IF EXISTS t_chkPara;
CREATE TABLE t_chkPara (
    id        INTEGER    PRIMARY KEY AUTOINCREMENT
                         UNIQUE,
    types     INTEGER,
    cmd CHAR (100),
    target    CHAR (200),
    comment   CHAR (200),
    sign      CHAR (2),
    FOREIGN KEY (
        types
    )
    REFERENCES t_chkScope (id) ON DELETE CASCADE
                             ON UPDATE CASCADE
                             MATCH [FULL]
);
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj id ', '$(id)', '逻辑地址', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cat /proc/version', 'NUC972_J1 version Linux (#117 PREEMPT Fri Aug 16 18:30:13 CST 2019) 3.10.101-g9482668-dirty ;;OR;; NUC972_J1 version Linux (#118 PREEMPT Mon Aug 19 11:44:25 CST 2019) 3.10.101-g57ebc18', '内核', 'OR');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj dev pro f101', '安全模式选择：不启用安全模式参数', '安全参数', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj esam', '主站证书 OK', 'ESAM', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj ip', '无线公网 主IP 10.158.243.224:8001', '698主IP', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj ip', '无线公网 备IP 10.158.243.224:8001', '698备IP', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj net-ip', '以太网接口 主IP 192.168.127.127:9027', '698以太网主IP', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj net-ip', '以太网接口 备IP 192.168.127.127:9027', '698以太网备IP', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj apn', 'sddl.cjxt.sd', '698apn', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj online-mode', 'gprs工作模式:1', '698GPRS工作模式', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj online-mode', '以太网工作模式:1', '698以太网工作模式', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj heart', '心跳周期:600 s', '698心跳', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj usr-pwd', '用户名： card       密码： card   电信APN： sddl.cjxt.sd', '电信APN用户名密码', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj event pro 3106', '[3106_para]=1 1 3 0 5 1 4320 10 5 1320 1760', '698停上电参数', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj para pro 4300', '是否允许主动上报:1', '698主动上报', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj rs485', '当前描述符(698)：波特率(6)，校验方式(2)，数据位(8)，停止位(1)，端口功能(0)', '维护485端口', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj bt', '3.55', '时钟电池电压', '>');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (1, 'cj bt', '3.8', '设备电池电压', '>');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (2, 'ps', 'cjmain', '698主程序', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (2, 'ps', 'cjcomm', '698上行程序', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (2, 'ps', 'cjdeal', '698下行程序', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nand/bin/cj', '3c5fac7b43a0c4bfb6f5966018cc4e3d', 'cj 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nand/bin/cjcomm', '762a2bf1d608d4b83ad4e9519b2074b4', 'cjcomm 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nand/bin/cjdeal', 'c0dc54925b3b905874acd8008b38dc95', 'cjdeal 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nand/bin/cjmain', '81edf8279f2e5ca3a4c0bcb507d2aac6', 'cjmain 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/lib376.2.so', 'd9c0f37f71ee17d6d7633adb842767ee', 'lib376.2.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/lib698.so', '3c5ff911dd02e406f461a6b04f0205a1', 'lib698.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/lib698Esam.so', '3a925cdc4dbf2b357d8ee2ddbe910197', 'lib698Esam.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/libAccess.so', 'e0389ba32bebf58662f3b4c4f0c9374b', 'libAccess.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/libBase.so', 'cc55240b25fea137331aa40868008cb0', 'libBase.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/libDlt645.so', 'd35eefa5adfb5bd3989d98177bf2e009', 'libDlt645.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/libGui.so', '0a4913bcbc42fb846e704c571603702c', 'libGui.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/libMq.so', '9c907cd1642a57b6fd2d3c483cda7f64', 'libMq.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/lib/libevent.so', '1723e427d0edb6dbeb6dc730b8eba63c', 'libevent.so 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/config/07DI_698OAD.cfg', '59775cbc40a0b3cd77ecabe7db23436d', '07DI_698OAD.cfg 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/config/OI_TYPE.cfg', 'e798ab2e3932179f1866dc423adbfd9c', 'OI_TYPE.cfg 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/config/device.cfg', 'f419e6df5e98ae71ca9772e67a3100ef', 'device.cfg 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/config/systema.cfg', '0d08b6ca1506500c32d2798090d16da7', 'systema.cfg 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/bin/mux.sh', '780c6d0feaae0f08ef2f64c7a4de9fc0', 'mux.sh 校验值', '=');
INSERT INTO t_chkPara (types, cmd, target, comment, sign) VALUES (3, 'md5sum /nor/bin/gsmMuxd', '2712bba63b7520765f79d76f6316fed2', 'gsmMuxd 校验值', '=');

-- 表：t_chkScope
DROP TABLE IF EXISTS t_chkScope;
CREATE TABLE t_chkScope (
    id    INTEGER   PRIMARY KEY AUTOINCREMENT
                    UNIQUE,
    types CHAR (50)
);
INSERT INTO t_chkScope (types) VALUES ('para');
INSERT INTO t_chkScope (types) VALUES ('pid');
INSERT INTO t_chkScope (types) VALUES ('chkSum');

-- 表：t_setpara
DROP TABLE IF EXISTS t_setpara;
CREATE TABLE t_setpara (
    id      INTEGER    PRIMARY KEY AUTOINCREMENT,
    name    CHAR (50),
    cmd     CHAR (100),
    comment CHAR (200),
	delay   INTEGER
);
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('setId', 'cj id ', '设置逻辑地址', '0');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('kill cjdeal', 'pkill cjdeal', '停止cjdeal', '2');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('init4', 'cj InIt 4', '恢复出厂默认参数', '3');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('set3106', 'cj event init 3106', '设置停上电参数', '3');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('setIp', 'cj ip 10.158.243.224:8001 10.158.243.224:8001', '设置无线公网主备IP及端口号', '0');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('setNetIp', 'cj net-ip 192.168.127.127:9027 192.168.127.127:9027', '设置以太网主备IP及端口号', '0');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('setApn', 'cj apn sddl.cjxt.sd', '设置无线公网APN', '0');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('setHeart', 'cj heart 600', '设置心跳周期', '0');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('setOnlineMode', 'cj online-mode 1 1', '设置上线模式', '0');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('setRs485', 'cj rs485 2 6 2 8 1 0', '设置 485-2 端口', '0');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('set3106Para', 'cj event set 3106 06 01 {1,4320,5,1,1320,1760}', '设置停上电默认参数', '2');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('setTelecom', 'cj usr-pwd card card sddl.cjxt.sd', '设置中国电信用户名密码及APN', '0');
INSERT INTO t_setpara (name, cmd, comment, delay) VALUES ('reboot', 'reboot', '重启终端', '0');

-- 表：t_apn_jibei
DROP TABLE IF EXISTS t_apn_jibei;

CREATE TABLE t_apn_jibei (
    id      INTEGER    PRIMARY KEY AUTOINCREMENT,
    cityId  INTEGER    REFERENCES t_city (id),
    vedorid INTEGER    REFERENCES t_mobile_vendor (id),
    ip      CHAR (50),
    port    CHAR (50),
    apn     CHAR (50),
    user    CHAR (100),
    pwd     CHAR (100) 
);

INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (1, 1, 1, '172.16.30.56', '9010', 'BDL8-QHD.HE', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (2, 1, 2, '192.168.9.1', '9010', 'qhdgdj1.heapn', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (3, 1, 3, '192.168.0.2', '9010', 'private.vpdn.he', 'lfgdj@lfgdj.vpdn.he', 'lfgdj');
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (4, 2, 1, '192.168.001.002', '9010', 'CDPOW8-CHD.HE', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (5, 2, 2, '192.168.010.002', '9010', 'L.WXPOS.HEAPN', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (6, 2, 3, '192.168.0.2', '9010', 'private.vpdn.he', 'lfgdj@lfgdj.vpdn.he', 'lfgdj');
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (7, 3, 1, '192.168.1.5', '9010', 'gdgs8-zjk.he', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (8, 3, 2, '192.168.254.245', '9010', 'zjgdj1.heapn', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (9, 3, 3, '192.168.0.2', '9010', 'private.vpdn.he', 'lfgdj@lfgdj.vpdn.he', 'lfgdj');
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (10, 4, 1, '172.29.1.5', '9010', 'PMON8-TAS.HE', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (11, 4, 2, '172.29.1.5', '9010', 'ydxx.ydoa.heapn', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (12, 4, 3, '192.168.0.2', '9010', 'private.vpdn.he', 'lfgdj@lfgdj.vpdn.he', 'lfgdj');
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (13, 5, 1, '211.143.102.138', '9010', 'POW8-LAF.HE', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (14, 5, 2, '192.168.0.10', '9010', 'LFGHD.YCCB.HEAPN', NULL, NULL);
INSERT INTO t_apn_jibei (id, cityId, vedorid, ip, port, apn, user, pwd) VALUES (15, 5, 3, '192.168.0.2', '9010', 'PRIVATE.VPDN.HE', 'LFGDJ@LFGDJ.VPDN.HE', 'LFGDJ');


-- 表：t_city
DROP TABLE IF EXISTS t_city;

CREATE TABLE t_city (
    id   INTEGER    PRIMARY KEY AUTOINCREMENT,
    name CHAR (100),
    code INTEGER
);

INSERT INTO t_city (id, name, code) VALUES (1, '秦皇岛', 1383);
INSERT INTO t_city (id, name, code) VALUES (2, '承德', 1358);
INSERT INTO t_city (id, name, code) VALUES (3, '张家口', 1397);
INSERT INTO t_city (id, name, code) VALUES (4, '唐山', 1382);
INSERT INTO t_city (id, name, code) VALUES (5, '廊坊', 1390);
INSERT INTO t_city (id, name, code) VALUES (6, '公网', 3706);

-- 表：t_mobile_vendor
DROP TABLE IF EXISTS t_mobile_vendor;

CREATE TABLE t_mobile_vendor (
    id   INTEGER   PRIMARY KEY AUTOINCREMENT,
    name CHAR (50) 
);

INSERT INTO t_mobile_vendor (id, name) VALUES (1, 'chinaMobile');
INSERT INTO t_mobile_vendor (id, name) VALUES (2, 'chinaUnicom');
INSERT INTO t_mobile_vendor (id, name) VALUES (3, 'chinaTelecom');

-- 视图：v_apnTbl
DROP VIEW IF EXISTS v_apnTbl;
CREATE VIEW v_apnTbl AS
    SELECT t.name,
           t.code,
           m.name,
           s.ip,
           s.port,
           s.apn,
           s.user,
           s.pwd
      FROM t_apn_jibei s
           LEFT JOIN
           t_city t ON s.cityId = t.id
           LEFT JOIN
           t_mobile_vendor m ON s.vedorid = m.id;
		   
-- 视图：v_chkPara
DROP VIEW IF EXISTS v_chkPara;
CREATE VIEW v_chkPara AS
    SELECT s.id,
           s.cmd,
           s.target,
           s.comment,
           t.types
      FROM t_chkPara s
           LEFT JOIN
           t_chkScope t ON s.types = t.id;

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
