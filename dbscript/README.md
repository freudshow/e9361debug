# sqlite3的建库脚本

~~建库工具~~

1. ~~可以使用`sqliteStudio`, 新建一个空数据库名为`user.db`, 然后将脚本复制到`sql`编辑器中运行即可.~~
2. ~~可以到[sqlite官网](https://sqlite.org/download.html) 下载最新的命令行工具 (本文成文时, 最新版本为3.34, 这里是[下载链接](https://sqlite.org/2020/sqlite-tools-win32-x86-3340000.zip)), 已下载好放到了本文同级目录下的`sqlitetools`文件夹里), 然后运行`sqlite3.exe user.db<userdb.create.sql`即可生成数据库.~~

## 生成数据库

现在数据库使用简单的`Sqlite3`数据库, 其文件名, 修改为`analog.db`, 并作为程序运行时唯一的数据库来源. 

建库方法:

- 方法1, 使用可视化的`Sqlite3`客户端, 比如`sqliteStudio`, `sqlite Expert`等, 将建库脚本的内容复制到可视化客户端的`sql`编辑器, 然后`Ctl+A`全选, 点击`运行`, 即可建库.
- 方法2, 可以到[sqlite官网](https://sqlite.org/download.html)下载最新的命令行工具 (本文成文时, 最新版本为3.36, 这里是[下载链接](https://sqlite.org/2020/sqlite-tools-win32-x86-3340000.zip)), 已下载好放到了本文同级目录下的`sqlitetools`文件夹里), 然后运行`sqlite3.exe analog.db < analog.create.sql`即可生成数据库.

