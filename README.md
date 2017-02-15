# dotnetGen_mysql
.NETCore + Mysql 生成器

本项目为生成器，用于快速创建和更新 .NETCore + Mysql 项目，非常合适敏捷开发；

优势：
 * 1、根据主键、唯一键、外键（1对1，1对多，多对多）生成功能丰富的数据库 SDK；
 * 2、避免随意创建表，严格把控数据库，有标准的ER图；
 * 3、统一规范数据库操作类与方法，一条心堆业务；

[下载生成器客户端试用](http://files.cnblogs.com/files/kellynic/%E7%94%9F%E6%88%90%E5%99%A8MySql.zip)

> MakeCode 已支持命令行，查看帮助可输入 cmd -> makecode ?
> C:/Users/Administrator/Desktop/生成器MySql/MakeCode 10.17.65.88:3066 -U root -P *** -D 数据库 -N 项目名 -O "X:/xxx/"
> 开发过程中，修改表后执行命令可以快速更新SDK；

共勉，一起学习QQ群：8578575

-----------------

生成的项目运行需要注意：

 * 修改 /admin/appsettings.json 里面的 redis 配置（缓存、Session 使用了 redis）

-----------------

| <font color=gray>功能对比</font> | [dotnetGen](https://github.com/2881099/dotnetGen) | [dotnetGen_sqlserver](https://github.com/2881099/dotnetGen_sqlserver) | [dotnetGen_mysql](https://github.com/2881099/dotnetGen_mysql) | [dotnetGen_postgresql](https://github.com/2881099/dotnetGen_postgresql) |
| ----------------: | -------------:| --------------------:| --------------: | -------------------: |
| windows            | √ | √ | √ | √ |
| linux              | - | - | √ | √ |
| 连接池             | √ | √ | √ | √ |
| 事务               | √ | √ | √ | √ |
| 表                 | √ | √ | √ | √ |
| 表关系(1对1)        | √ | √ | √ | √ |
| 表关系(1对多)       | √ | √ | √ | √ |
| 表关系(多对多)      | √ | √ | √ | √ |
| 表主键             | √ | √ | √ | √ |
| 表唯一键           | √ | √ | √ | √ |
| 存储过程           | √ | - | - | - |
| 视图               | - | - | √ | √ |
| 类型映射           | √ | √ | √ | √ |
| 枚举               | - | - | √ | √ |
| 自定义类型         | - | - | - | √ |
| 数组               | - | - | - | √ |
| xml               | - | - | - | - |
| json              | - | - | - | √ |
| 命令行生成         | - | - | √ | √ |
| RESTful           | - | - | √ | √ |
| 后台管理功能       | √ | - | √ | √ |