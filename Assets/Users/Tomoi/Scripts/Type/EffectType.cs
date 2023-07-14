public enum EffectType 
{
    //エフェクトの判定、生成用に合成したenum
    //既存のenum同士で同じ定数を使用しないように注意
    //魔法は0スタート
    Magic_None = 0,
    Magic_Fire = 1,
    Magic_Water = 2,
    Magic_Ice = 3,
    Magic_Wind = 4,
    
    //ステージオブジェクトは100スタート
    StageObject_None = 100,           //何もない
    StageObject_Magma = 101,          //マグマ
    StageObject_Grassland = 102,      //草原
    StageObject_Wood = 103,           //木
    StageObject_Monster = 104,        //モンスター
    StageObject_Ice = 105,            //氷
    StageObject_Pond = 106,           //池
    StageObject_Abyss = 107,          //深淵
    StageObject_Flame = 108,          //火炎
    StageObject_Rock = 109,           //石
    StageObject_Key = 110,           //鍵
    StageObject_MagicCircle = 111    //魔法陣
    
    //その他特殊系は900スタート
}
