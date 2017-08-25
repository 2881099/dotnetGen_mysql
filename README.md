# dotnetGen_mysql
.NETCore 2.0 + Mysql 生成器

本项目为生成器，用于快速创建和更新 .NETCore 2.0 + Mysql 项目，非常合适敏捷开发；

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


### 测试数据库

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