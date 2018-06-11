# dotnetGen_mysql
.NETCore 2.1 + Mysql 生成器

本项目为生成器，用于快速创建和更新 .NETCore 2.1 + Mysql 项目，非常合适敏捷开发；

优势：
 * 1、根据主键、唯一键、外键（1对1，1对多，多对多）生成功能丰富的数据库 SDK；
 * 2、避免随意创建表，严格把控数据库，有标准的ER图；
 * 3、统一规范数据库操作类与方法，一条心堆业务；

[下载生成器客户端试用](http://files.cnblogs.com/files/kellynic/%E7%94%9F%E6%88%90%E5%99%A8MySql.zip)

> 或者 dotnet tool install -g GenMy

> 查看帮助可输入 cmd -> GenMy ?

共勉，一起学习QQ群：8578575

-----------------

生成的项目运行需要注意：

 * 修改 /admin/appsettings.json 里面的 redis 配置（缓存、Session 使用了 redis）

-----------------


| <font color=gray>功能对比</font> | [dotnetGen_mysql](https://github.com/2881099/dotnetGen_mysql) | [dotnetGen_postgresql](https://github.com/2881099/dotnetGen_postgresql) |
| ----------------: | --------------: | -------------------: |
| windows            | √ | √ |
| linux              | √ | √ |
| 连接池             | √ | √ |
| 事务               | √ | √ |
| 多数据库            | √ | - |
| 表                 | √ | √ |
| 表关系(1对1)        | √ | √ |
| 表关系(1对多)       | √ | √ |
| 表关系(多对多)      | √ | √ |
| 表主键             | √ | √ |
| 表唯一键           | √ | √ |
| 存储过程           | - | - |
| 视图               | √ | √ |
| 类型映射           | √ | √ |
| 枚举               | √ | √ |
| 自定义类型         | - | √ |
| 数组               | - | √ |
| xml               | - | - |
| json              | - | √ |
| gis               | - | √ |
| 命令行生成         | √ | √ |
| RESTful           | √ | √ |
| 后台管理功能       | √ | √ |


### 测试数据库

![](20170825201740.png)

```sql
SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for song
-- ----------------------------
DROP TABLE IF EXISTS `song`;
CREATE TABLE `song` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(128) COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '歌名',
  `url` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '地址',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for song_tag
-- ----------------------------
DROP TABLE IF EXISTS `song_tag`;
CREATE TABLE `song_tag` (
  `song_id` int(11) NOT NULL COMMENT '歌曲',
  `tag_id` int(11) NOT NULL COMMENT '标签',
  PRIMARY KEY (`song_id`,`tag_id`),
  KEY `fk_song_tag_tag_1` (`tag_id`),
  CONSTRAINT `fk_song_tag_song_1` FOREIGN KEY (`song_id`) REFERENCES `song` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_song_tag_tag_1` FOREIGN KEY (`tag_id`) REFERENCES `tag` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for tag
-- ----------------------------
DROP TABLE IF EXISTS `tag`;
CREATE TABLE `tag` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `parent_id` int(11) DEFAULT NULL COMMENT '父标签',
  `name` varchar(128) COLLATE utf8_unicode_ci DEFAULT NULL COMMENT '名称',
  PRIMARY KEY (`id`),
  KEY `fk_tag_tag_1` (`parent_id`),
  CONSTRAINT `fk_tag_tag_1` FOREIGN KEY (`parent_id`) REFERENCES `tag` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for topic
-- ----------------------------
DROP TABLE IF EXISTS `topic`;
CREATE TABLE `topic` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(255) DEFAULT NULL COMMENT '标题',
  `cardtype` enum('视频','图文01','图文02','链接') DEFAULT NULL COMMENT '卡片类型',
  `carddata` text COMMENT '卡片渲染数据',
  `content` text COMMENT '内容',
  `clicks` bigint(20) unsigned DEFAULT NULL COMMENT '点击次数',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_time` datetime DEFAULT NULL COMMENT '修改时间',
  `order_time` datetime DEFAULT NULL COMMENT '排序时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```



# 项目结构介绍

## Module

	所有业务接口约定在 Module 划分开发

	Module/Admin
	生成的后台管理模块，http://localhost:5001/module/Admin 可访问

	Module/Test
	生成的测试模块

## WebHost

	WebHost 编译的时候，会将 Module/* 所需文件复制到当前目录
	WebHost 只当做主引擎运行时按需加载相应的 Module

## Common

	框架最基层依赖（为了持续更新，建议开发者不用动）
	好用：WorkQueue.cs(线程级消息队列)、Socket/*(服务端与客户端封装)、Robot.cs(定时器)

## Infrastructure

	Module 里面每个子模块的依赖所需

#### xx.db

	包含一切数据库操作的封装
	xx.Model(实体映射 命名：表名Info)
	xx.BLL(静态方法封装 命名：表名)
	xx.DAL(数据访问 命名：表名)
	生成名特征取数据库名首字母大写(如: 表 test 对应 xx.Model.TestInfo、xx.BLL.Test、xx.DAL.Test)

	数据库设计命名习惯：所有命名(小写加下划线)、外键字段(对应主表名_主表PK名)
	外键不支持组合字段，仅支持主键作为外键(ps: 不支持唯一键作为外键)
	修改数据库后，双击“./GenPg只更新db.bat”可快速覆盖，所有类都使用 partial，方便代码扩展亦不会被二次生成覆盖

# 数据库相关方法

	假设下面的代码都使用了 using xx.BLL; using xx.Model;

## 添加记录

```csharp
	// 如有 create_time 字段并且类型为日期，内部会初始化
	表名Info newitem1 = 表名.Insert(Title: "添加的标题", Content: "这是一段添加的内容");
	表名Info newitem2 = 表名.Insert(new 表名Info { Title = "添加的标题", Content = "这是一段添加的内容" });
```

## 添加记录(批量)

```csharp
	List<表名Info> newitems1 = 表名.Insert(new [] {
		new 表名Info { Title = "添加的标题1", Content = "这是一段添加的内容1" },
		new 表名Info { Title = "添加的标题2", Content = "这是一段添加的内容2" }
	});
```

## 更新记录

```csharp
	// 更新 id = 1 所有字段
	表名Info newitem1 = 表名.Update(Id: 1, Title: "添加的标题", Content: "这是一段添加的内容", Clicks: 1);
	表名Info newitem2 = 表名.Insert(new 表名Info { Id: 1, Title = "添加的标题", Content = "这是一段添加的内容", Clicks = 1 });
	// 更新 id = 1 指定字段
	表名.UpdateDiy(1).SetTitle("修改后的标题").SetContent("修改后的内容").SetClicks(1).ExecuteNonQuery();
	// update 表名 set clicks = clicks + 1 where id = 1
	表名.UpdateDiy(1).SetClicksIncrement(1).ExecuteNonQuery();
	// xx.Model 层也有 UpdateDiy，即 new 表名Info { Id = 1 }.UpdateDiy.SetClicks(1).ExecuteNonQuery();
```

## 更新记录(批量)

```csharp
	//先查找 clicks 在 0 - 100 的记录
	List<表名Info> newitems1 = 表名.Select.WhereClicksRange(0, 100).ToList();
	// update 表名 set clicks = clicks + 1 where id in (newitems1所有id)
	newitems1.UpdateDiy().SetClicksIncrement(1).ExecuteNonQuery();
```

## 删除记录

```csharp
	// 删除 id = 1 的记录
	表名.Delete(1);
```

## 按主键/唯一键获取单条记录

> appsettings可配置缓存时间，以上所有增、改、删都会删除缓存保障同步

```csharp
	//按主键获取
	UserInfo user1 = BLL.User.GetItem(1);
	//按唯一键
	UserInfo user2 = BLL.User.GetItemByUsername("2881099@qq.com");
	// 返回 null 或 UserInfo
```

## 查询(核心)

```csharp
	//BLL.表名.Select 是一个链式查询对象，几乎支持所有查询，包括 group by、inner join等等，最终 ToList ToOne 执行 sql
	List<UserInfo> users1 = BLL.User.Select.WhereUsername("2881099@qq.com").WherePassword("******").WhereStatus(正常).ToList();
	//返回 new List<UserInfo>() 或 有元素的 List，永不返回 null
```

## 事务

```csharp
//错误会回滚，事务内支持所有生成的同步方法（不支持生成对应的Async方法）
SqlHelper.Transaction(() => {
	if (this.Balance.UpdateDiy.SetAmountIncrement(-num).ExecuteNonQuery() <= 0) throw new Exception("余额不足");
	extdata["amountNew"] = this.Balance.Amount.ToString();
	extdata["balanceChangelogId"] = RedisHelper.NewMongodbId();
	order = this.AddBet_order(Amount: 1, Count: num, Count_off: num, Extdata: extdata, Status: Et_bet_order_statusENUM.NEW, Type: Et_bet_tradetypeENUM.拆分);
});
```


# 生成规则

## 不会生成

* 没有主键，不会生成 增、改、删 方法
* 有自增字段，不会生成 批量 Insert 方法

## 会生成

* 有外键，会生成
	> new 主键表Info().Addxxx、new 主键表Model().Obj_外键表s

	> new 外键表Info().Obj_主键表、外键表.DeleteBy外键、外键表.Select.Where外键
* 多对多，会生成
	> new 表1Info().Flag表2、new 表1Info().UnFlag表2、new 表1Info().Obj_表2s、表1.Select.Where表2

	> new 表2Info().Flag表1、new 表2Info().UnFlag表1、new 表2Info().Obj_表1s、表2.Select.Where表1
* 字段类型 point，会生成
	> 表.Select.Where字段MbrContains(查找地理位置多少米范围内的记录，距离由近到远排序)
* 字段类型 string相关并且长度 <= 300，会生成
	> 表.Select.Where字段Like
* 99%的数据类型被支持