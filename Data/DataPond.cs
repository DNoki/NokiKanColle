using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NokiKanColle.Data
{
    public class DataPond
    {
        public class DataJudgePonds
        {
            // 母港页面
            public static readonly DataJudge 母港1 = new DataJudge("773,449 @ E7EDEA|2EA4A7|209FA2|2EA4A7|E7EDEA");
            public static readonly DataJudge 母港2 = new DataJudge("662,41 @ 1B6112");
            public static readonly DataJudge 远征回港 = new DataJudge("523,26 @ 36AFA7|25ACA9|18AFB1|11B0B3|17B6B9|26BEBE");
            public static readonly DataJudge 远征归来1 = new DataJudge("196,235 @ FFFDFC|C9D1D0|698788|698788");
            public static readonly DataJudge 远征归来2 = new DataJudge("22,345 @ BCBFC1|626C73|ACAFB3|C7C8CA");

            public static readonly DataJudge 左侧返回母港1 = new DataJudge("75,270 @ F3F4EC|ACACA7|3A3A35|B9BAB5");
            public static readonly DataJudge 左侧返回母港2 = new DataJudge("75,270 @ F3F0E4|B1AEA0|3C3422|BFAF9B");

            // 补给页面
            public static readonly DataJudge 补给页 = new DataJudge("646,179 @ E2E4D6|818375|797B6D|CCCEC0|EFF1E3|9A9C8E");
            public static readonly DataJudge 补给队伍1 = new DataJudge("146,119 @ 23A0A1|D4E1DE|FFF6F2|D4E1DE");
            public static readonly DataJudge 补给队伍2 = new DataJudge("178,119 @ 23A0A1|D4E1DE|FFF6F2|53ADAD");
            public static readonly DataJudge 补给队伍3 = new DataJudge("206,118 @ 53ADAD|BCD6D4|FFF6F2|BCD6D4");
            public static readonly DataJudge 补给队伍4 = new DataJudge("235,117 @ 86BFBE|F5F1ED|53ADAD|77B9B9");
            public static readonly DataJudge 补给舰娘正在远征 = new DataJudge("274,167 @ 9895C5|313057|9897BA|A2A0CA");
            public static readonly DataJudge 补给舰娘存活List = new DataJudge(new List<DataJudge>() {
                new DataJudge("326,166 @ C7C0B1|B2AB9C|A69F90|CFC8B9"),
                new DataJudge("326,217 @ C7C0B1|B2AB9C|A69F90|CFC8B9"),
                // 于171118修改UI数据
                new DataJudge("326,268 @ C7C0B1|B2AB9C|A69F90|CFC8B9"),
                new DataJudge("326,319 @ C7C0B1|B2AB9C|A69F90|CFC8B9"),
                new DataJudge("326,370 @ C7C0B1|B2AB9C|A69F90|CFC8B9"),
                new DataJudge("326,421 @ C7C0B1|B2AB9C|A69F90|CFC8B9"),
            });
            public static readonly DataJudge 补油判定List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("516,166 @ 36FF00"),
                new DataJudge("516,217 @ 36FF00"),
                new DataJudge("516,269 @ 36FF00"),
                new DataJudge("516,321 @ 36FF00"),
                new DataJudge("516,373 @ 36FF00"),
                new DataJudge("516,425 @ 36FF00")
            });
            public static readonly DataJudge 补弹判定List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("587,166 @ 36FF00"),
                new DataJudge("587,217 @ 36FF00"),
                new DataJudge("587,269 @ 36FF00"),
                new DataJudge("587,321 @ 36FF00"),
                new DataJudge("587,373 @ 36FF00"),
                new DataJudge("587,425 @ 36FF00")
            });
            public static readonly DataJudge 全舰补给 = new DataJudge("116,122 @ F2C7B2|F6DCD4|F5DAD2");
            public static readonly DataJudge 非全舰补给 = new DataJudge("116,122 @ 9B9587|9B9587|9B9587");
            /*
            
            
            public static readonly DataJudge 母 = new DataJudge("662 @ 1B6112");
            public static readonly DataJudge 母 = new DataJudge("662 @ 1B6112");
            */
            // 出击页面
            public static readonly DataJudge 出击选择 = new DataJudge("140,77 @ 1EAAAB|1CB2B5|1DBDC0|1EAAAC|1EAAAC|208F90");
            public static readonly DataJudge 出击详细 = new DataJudge("605,76 @ 1B4C50|1DBDC0|1D2529|1DBDC0|1C7579", "出击详细");
            public static readonly DataJudge 出击舰队选择 = new DataJudge("637,130 @ 716E6D|FFF6F2|3A3A3A|E1D9D6", "出击舰队选择");

            // 远征页面
            public static readonly DataJudge 远征选择 = new DataJudge("520,110 @ E2D4B2|CCCDA0|EABBC8|F2C8D6|D5ABAA|8B492A");
            public static readonly DataJudge 远征舰队选择 = new DataJudge("669,126 @ 3E3C3C|8F8B89|FFF6F2|817E7C");//综合力
            public static readonly DataJudge 远征图List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("126,444 @ EE9570|FDF7F4|EA7D4A|EA7D4A|E76110|EA7D4A"),
                new DataJudge("191,446 @ FFFFFF|E76110|F2B69F|EFA081|EC895E|E76110"),
                new DataJudge("250,447 @ E7C5A5|FFDFB2|CB6530|BC6952|FADDDF|DBC0AA"),
                new DataJudge("302,446 @ FFDFB7|AF6E49|BB7556|FFCEAB|FFB07E|C06424"),
                new DataJudge("362,446 @ AD724D|B2724A|FFD8AB|FFAC78|D06729|D45E19")
            });
            public static readonly DataJudge 远征决定List = new DataJudge(new List<DataJudge>() {
                new DataJudge("667,443 @ 3B7662|055038|006D5E|FFF6F2|FFF6F2|E9E7E2"),
                new DataJudge("667,443 @ A24E2F|8D2918|6E1912|FFF6F2|FFF6F2|EFDFDB"),
                //new DataJudge("670,442 @ 779298|F1F9FF|FBF1F7|DBC9D2|8DA2A6|C8E6EF"), // 2017.10.18更新应对
                //new DataJudge("670,442 @ 819A99|EEFFF4|EDF9E0|D0D8BA|8BAEA2|C5EBE7") // 2017.10.18更新应对
            });
            public static readonly DataJudge 远征中止与归还List = new DataJudge(new List<DataJudge>() {
                new DataJudge("671,445 @ E1A49F|E4AEA9|D0BEB1|FBFFF0|FFFEF2|CEA6A2"),
                new DataJudge("694,442 @ 4E120D|711709|AD795F|F4F8DA"),
                //new DataJudge("671,445 @ 9F3D39|BA7A74|FFF3EF|FFF1F1|9A4848|AD4040"),// 2017.10.18更新应对
                //new DataJudge("694,442 @ F3AB5E|FFB668|FFEEAE|FFD9A4"),
            });
            public static readonly DataJudge 远征详细 = new DataJudge("622,77 @ 1D5958|1D9EA0|1D7778|202A28|1DBDC0|1D6363");
            public static readonly DataJudge 远征开始灰 = new DataJudge("609,440 @ 303030|727272|C7C7C7");// 2017.3.17 更新UI
            public static readonly DataJudge 远征开始List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("609,440 @ 074B69|999D97|EEFFFD"),
                new DataJudge("609,440 @ 6C2F09|BD918D|FFF0E6"),
            });
            public static readonly DataJudge 远征即将归还 = new DataJudge("743,392 @ 969189|969189|969189,D2CCC2|D2CCC2|D2CCC2");
            public static readonly DataJudge 远征舰队选择队伍List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("389,113 @ 23A0A1","远征舰队选择队伍2"),
                new DataJudge("421,116 @ 23A0A1","远征舰队选择队伍3"),
                new DataJudge("452,115 @ 23A0A1","远征舰队选择队伍4")});// 2017.3.17 更新UI

            // 出击前补给状态
            public static readonly DataJudge 舰队未补油1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("489,169 @ 333333|333333"),
                new DataJudge("489,219 @ 333333|333333"),
                new DataJudge("489,269 @ 333333|333333"),
                new DataJudge("489,319 @ 333333|333333"),
                new DataJudge("489,369 @ 333333|333333"),
                new DataJudge("489,419 @ 333333|333333")
            });
            public static readonly DataJudge 舰队未补油2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("489,169 @ FFDE00|FFDE00"),
                new DataJudge("489,219 @ FFDE00|FFDE00"),
                new DataJudge("489,269 @ FFDE00|FFDE00"),
                new DataJudge("489,319 @ FFDE00|FFDE00"),
                new DataJudge("489,369 @ FFDE00|FFDE00"),
                new DataJudge("489,419 @ FFDE00|FFDE00")
            });
            public static readonly DataJudge 舰队未补弹1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("511,169 @ 333333|333333"),
                new DataJudge("511,219 @ 333333|333333"),
                new DataJudge("511,269 @ 333333|333333"),
                new DataJudge("511,319 @ 333333|333333"),
                new DataJudge("511,369 @ 333333|333333"),
                new DataJudge("511,419 @ 333333|333333")
            });
            public static readonly DataJudge 舰队未补弹2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("511,169 @ FFDE00|FFDE00"),
                new DataJudge("511,219 @ FFDE00|FFDE00"),
                new DataJudge("511,269 @ FFDE00|FFDE00"),
                new DataJudge("511,319 @ FFDE00|FFDE00"),
                new DataJudge("511,369 @ FFDE00|FFDE00"),
                new DataJudge("511,419 @ FFDE00|FFDE00")
            });
            // 出击前破损状态
            public static readonly DataJudge 出击大破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("465,150 @ 99403B|8D3834"),
                new DataJudge("465,200 @ 99403B|8D3834"),
                new DataJudge("465,250 @ 99403B|8D3834"),
                new DataJudge("465,300 @ 99403B|8D3834"),
                new DataJudge("465,350 @ 99403B|8D3834"),
                new DataJudge("465,400 @ 99403B|8D3834")
            });
            public static readonly DataJudge 出击大破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("470,136 @ A94B46"),
                new DataJudge("470,186 @ A94B46"),
                new DataJudge("470,236 @ A94B46"),
                new DataJudge("470,286 @ A94B46"),
                new DataJudge("470,336 @ A94B46"),
                new DataJudge("470,386 @ A94B46")
            });
            public static readonly DataJudge 出击黄脸大破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("465,150 @ A94530|A14029"),
                new DataJudge("465,200 @ A94530|A14029"),
                new DataJudge("465,250 @ A94530|A14029"),
                new DataJudge("465,300 @ A94530|A14029"),
                new DataJudge("465,350 @ A94530|A14029"),
                new DataJudge("465,400 @ A94530|A14029")
            });
            public static readonly DataJudge 出击黄脸大破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("470,136 @ BD5034"),
                new DataJudge("470,186 @ BD5034"),
                new DataJudge("470,236 @ BD5034"),
                new DataJudge("470,286 @ BD5034"),
                new DataJudge("470,336 @ BD5034"),
                new DataJudge("470,386 @ BD5034")
            });
            public static readonly DataJudge 出击红脸大破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("465,150 @ A93430|A12D29"),
                new DataJudge("465,200 @ A93430|A12D29"),
                new DataJudge("465,250 @ A93430|A12D29"),
                new DataJudge("465,300 @ A93430|A12D29"),
                new DataJudge("465,350 @ A93430|A12D29"),
                new DataJudge("465,400 @ A93430|A12D29")
            });
            public static readonly DataJudge 出击红脸大破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("470,136 @ BD3734"),
                new DataJudge("470,186 @ BD3734"),
                new DataJudge("470,236 @ BD3734"),
                new DataJudge("470,286 @ BD3734"),
                new DataJudge("470,336 @ BD3734"),
                new DataJudge("470,386 @ BD3734")
            });
            public static readonly DataJudge 出击中破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("465,144 @ FDBA5C|FDBA5C"),
                new DataJudge("465,194 @ FDBA5C|FDBA5C"),
                new DataJudge("465,244 @ FDBA5C|FDBA5C"),
                new DataJudge("465,294 @ FDBA5C|FDBA5C"),
                new DataJudge("465,344 @ FDBA5C|FDBA5C"),
                new DataJudge("465,394 @ FDBA5C|FDBA5C")
            });
            public static readonly DataJudge 出击中破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("468,134 @ FFBC5D|FDBA5C"),
                new DataJudge("468,184 @ FFBC5D|FDBA5C"),
                new DataJudge("468,234 @ FFBC5D|FDBA5C"),
                new DataJudge("468,284 @ FFBC5D|FDBA5C"),
                new DataJudge("468,334 @ FFBC5D|FDBA5C"),
                new DataJudge("468,384 @ FFBC5D|FDBA5C")
            });
            public static readonly DataJudge 出击黄脸中破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("465,144 @ FBA94B|FBA84A"),
                new DataJudge("465,194 @ FBA94B|FBA84A"),
                new DataJudge("465,244 @ FBA94B|FBA84A"),
                new DataJudge("465,294 @ FBA94B|FBA84A"),
                new DataJudge("465,344 @ FBA94B|FBA84A"),
                new DataJudge("465,394 @ FBA94B|FBA84A")
            });
            public static readonly DataJudge 出击黄脸中破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("468,134 @ FDA848|FBA446"),
                new DataJudge("468,184 @ FDA848|FBA446"),
                new DataJudge("468,234 @ FDA848|FBA446"),
                new DataJudge("468,284 @ FDA848|FBA446"),
                new DataJudge("468,334 @ FDA848|FBA446"),
                new DataJudge("468,384 @ FDA848|FBA446")
            });
            public static readonly DataJudge 出击红脸中破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("465,144 @ FB984B|FB954A"),
                new DataJudge("465,194 @ FB984B|FB954A"),
                new DataJudge("465,244 @ FB984B|FB954A"),
                new DataJudge("465,294 @ FB984B|FB954A"),
                new DataJudge("465,344 @ FB984B|FB954A"),
                new DataJudge("465,394 @ FB984B|FB954A")
            });
            public static readonly DataJudge 出击红脸中破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("468,134 @ FD9248|F98C45"),
                new DataJudge("468,184 @ FD9248|F98C45"),
                new DataJudge("468,234 @ FD9248|F98C45"),
                new DataJudge("468,284 @ FD9248|F98C45"),
                new DataJudge("468,334 @ FD9248|F98C45"),
                new DataJudge("468,384 @ FD9248|F98C45")
            });
            public static readonly DataJudge 出击小破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("471,139 @ F7E25A|F6E159"),
                new DataJudge("471,189 @ F7E25A|F6E159"),
                new DataJudge("471,239 @ F7E25A|F6E159"),
                new DataJudge("471,289 @ F7E25A|F6E159"),
                new DataJudge("471,339 @ F7E25A|F6E159"),
                new DataJudge("471,389 @ F7E25A|F6E159")
            });
            public static readonly DataJudge 出击小破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("473,154 @ ECD856"),
                new DataJudge("473,204 @ ECD856"),
                new DataJudge("473,254 @ ECD856"),
                new DataJudge("473,304 @ ECD856"),
                new DataJudge("473,354 @ ECD856"),
                new DataJudge("473,404 @ ECD856")
            });
            public static readonly DataJudge 出击黄脸小破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("471,139 @ F7BF42|F6BD3F"),
                new DataJudge("471,189 @ F7BF42|F6BD3F"),
                new DataJudge("471,239 @ F7BF42|F6BD3F"),
                new DataJudge("471,289 @ F7BF42|F6BD3F"),
                new DataJudge("471,339 @ F7BF42|F6BD3F"),
                new DataJudge("471,389 @ F7BF42|F6BD3F")
            });
            public static readonly DataJudge 出击黄脸小破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("473,154 @ EFB43C"),
                new DataJudge("473,204 @ EFB43C"),
                new DataJudge("473,254 @ EFB43C"),
                new DataJudge("473,304 @ EFB43C"),
                new DataJudge("473,354 @ EFB43C"),
                new DataJudge("473,404 @ EFB43C")
            });
            public static readonly DataJudge 出击红脸小破1List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("471,139 @ F7A542|F6A13F"),
                new DataJudge("471,189 @ F7A542|F6A13F"),
                new DataJudge("471,239 @ F7A542|F6A13F"),
                new DataJudge("471,289 @ F7A542|F6A13F"),
                new DataJudge("471,339 @ F7A542|F6A13F"),
                new DataJudge("471,389 @ F7A542|F6A13F")
            });
            public static readonly DataJudge 出击红脸小破2List = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("473,154 @ EF973C"),
                new DataJudge("473,204 @ EF973C"),
                new DataJudge("473,254 @ EF973C"),
                new DataJudge("473,304 @ EF973C"),
                new DataJudge("473,354 @ EF973C"),
                new DataJudge("473,404 @ EF973C")
            });

            // 入渠页
            public static readonly DataJudge 入渠页 = new DataJudge("136,82 @ 374649|23A0A4|1DBDC0|25989A", "入渠页");
            public static readonly DataJudge 入渠舰船选择 = new DataJudge("424,83 @ 2B2524|246161|1DBDC0|2A2422", "舰船选择");

            //public static readonly DataJudge 舰队未补 = new DataJudge("662 @ 1B6112");
            //public static readonly DataJudge 母 = new DataJudge("662 @ 1B6112");
            /*
            public static readonly DataJudge 远征舰队选择队伍 = new DataJudge(new List<DataJudge>()
            {
                new DataJudge("662 @ 1B6112"),
            });
            */
        }
        public class DataClickPonds
        {
            // 母港页
            public static readonly DataClick 母港出击 = new DataClick("196,210|152,232|149,272|191,305|239,277|240,239|223,218");
            public static readonly DataClick 左侧返回母港 = new DataClick("60,223|86,292");
            public static readonly DataClick 舰娘立绘 = new DataClick("564,181|712,376");
            public static readonly DataClick 补给 = new DataClick("75,189|46,218|73,248|108,221");
            // 出击选择页
            public static readonly DataClick 远征 = new DataClick("669,128|615,155|590,215|625,276|687,294|733,270|747,244|754,186|727,151");
            // 补给页
            public static readonly DataClick 一括补给 = new DataClick("113,116|122,125");
            public static readonly DataClick 补给队伍1 = new DataClick("142,114|153,124");
            public static readonly DataClick 补给队伍2 = new DataClick("172,114|184,124");
            public static readonly DataClick 补给队伍3 = new DataClick("203,114|214,124");
            public static readonly DataClick 补给队伍4 = new DataClick("232,114|244,124");
            /*
            public static readonly DataClick 母港出 = new DataClick("");
            public static readonly DataClick 母港出 = new DataClick("");
            */
            // 远征页
            public static readonly DataClick 远征海域List = new DataClick(new List<DataClick>()
            {
                new DataClick("123,442|139,448"),
                new DataClick("196,443|210,449"),
                new DataClick("247,438|263,447"),
                new DataClick("300,438|316,446"),
                new DataClick("360,437|377,446"),
                // 2017.10.18更新应对
                //new DataClick("137,419|117,428|114,450|157,451|155,439"),
                //new DataClick("174,439|221,451"),
                //new DataClick("238,436|278,451"),
                //new DataClick("288,435|330,451"),
                //new DataClick("348,429|389,451"),
            });
            public static readonly DataClick 远征项List = new DataClick(new List<DataClick>()
            {
                new DataClick("143,168|469,177"),
                new DataClick("143,199|469,207"),
                new DataClick("143,229|469,237"),
                new DataClick("143,259|469,267"),
                new DataClick("143,289|469,297"),
                new DataClick("143,319|469,327"),
                new DataClick("143,349|469,357"),
                new DataClick("143,379|469,387")
            });
            public static readonly DataClick 远征决定 = new DataClick("605,429|596,443|605,459|765,456|774,446|766,431");
            //public static readonly DataClick 远征决定 = new DataClick("638,427|629,443|638,461|717,461|727,443|717,427");// 2017.10.18更新应对
            public static readonly DataClick 远征开始 = new DataClick("537,434|690,455");
            public static readonly DataClick 远征舰队选择List = new DataClick(new List<DataClick>()
            {
                new DataClick("386,110|399,119"),
                new DataClick("418,109|430,121"),
                new DataClick("448,109|462,122")
            });
            /*
             public static readonly DataClick 远征舰队选择List = new DataClick(new List<DataClick>()
            {
                new DataClick(""),
            });
             */

            // 出击
            public static readonly DataClick 出击决定 = new DataClick("614,434|757,456", "出击决定");
            public static readonly DataClick 追击判定 = new DataClick(new List<DataClick>()
            {
                new DataClick("285,208|239,236|249,270|298,277|341,246|331,209","追撃せず"),
                new DataClick("497,212|460,235|467,266|513,272|549,245|543,214","夜戦突入")
            });

            public static readonly DataClick 出击阵型5 = new DataClick(new List<DataClick>()
            {
                new DataClick("400,173|493,195","单纵阵"),
                new DataClick("530,173|623,195","复纵阵"),
                new DataClick("662,173|755,195","轮型阵"),
                new DataClick("474,336|557,348","梯形阵"),
                new DataClick("605,336|689,348","单横阵"),
            });
            public static readonly DataClick 出击阵型6 = new DataClick(new List<DataClick>()
            {
                new DataClick("400,173|493,195","单纵阵"),
                new DataClick("530,173|623,195","复纵阵"),
                new DataClick("662,173|755,195","轮型阵"),
                new DataClick("407,336|486,348","梯形阵"),
                new DataClick("537,336|618,348","单横阵"),
                new DataClick("669,336|748,348","警戒阵"),
            });
        }
    }
}
