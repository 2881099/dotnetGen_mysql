# dotnetGen_mysql
.NETCore + Mysql 生成器

开发初期，它可以选择数据库表创建项目；

开发阶段，它可以即时更新项目中数据层 SDK，提供业务开发基础；

优势：
 * 1、根据主键、唯一键、外键（1对1，1对多，多对多）生成功能丰富的数据库 SDK；
 * 2、避免随意创建表，严格把控数据库，有标准的ER图；
 * 3、统一规范数据库操作类与方法，一条心堆业务；

[下载生成器客户端试用](http://files.cnblogs.com/files/kellynic/%E7%94%9F%E6%88%90%E5%99%A8MySql.zip)

如果觉得官方服务器慢，可以选择自己部署：
 * 运行环境：.net 2.0+
 * 安装 ServerWinService 服务，启动前设置服务端口
 * 配置 MakeCode 服务器，向开发者发送 MakeCode 生成器

共勉，一起学习QQ群：8578575

-----------------

生成的项目运行大概需要注意以下几点：

 * 修改 /admin/appsettings.json 里面的 redis 配置（缓存、Session 使用了 redis）
 * 修改 nuget 源：（使用了 Swashbuckle.Swagger 6.0.0-preview-0035）

	```xml
		<add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
		<add key="nuget.org" value="http://api.nuget.org/v3/index.json" />
		<add key="Ahoy Preview MyGet" value="https://www.myget.org/F/domaindrivendev/api/v3/index.json" />
	```
