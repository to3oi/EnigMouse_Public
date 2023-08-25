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
    
    //発動する魔法
    //Fire
    Magic_Fire_initCircle = 11,
    //initChargeはDestroyするので未使用
    //Magic_Fire_initCharge = 12,
    Magic_Fire_defaultCircle = 13,
    Magic_Fire_igniteParticle = 14,
    Magic_Fire_releaseCircle = 15,
    
    //Ice
    Magic_Ice_initCircle = 21,
    //initChargeはDestroyするので未使用
    //Magic_Ice_initCharge = 22,
    Magic_Ice_defaultCircle = 23,
    Magic_Ice_igniteParticle = 24,
    Magic_Ice_releaseCircle = 25,
    
    //Water
    Magic_Water_initCircle = 31,
    //initChargeはDestroyするので未使用
    //Magic_Water_initCharge = 32,
    Magic_Water_defaultCircle = 33,
    Magic_Water_igniteParticle = 34,
    Magic_Water_releaseCircle = 35,
    
    //Wind
    Magic_Wind_initCircle = 41,
    //initChargeはDestroyするので未使用
    //Magic_Wind_initCharge = 42,
    Magic_Wind_defaultCircle = 43,
    Magic_Wind_igniteParticle = 44,
    Magic_Wind_releaseCircle = 45,
    
    
    //ステージオブジェクトは100スタート
    StageObject_None = 100,             //何もない
    StageObject_Magma = 101,            //マグマ
    StageObject_Grassland = 102,        //草原
    StageObject_Wood = 103,             //木
    StageObject_Monster = 104,          //モンスター
    StageObject_Ice = 105,              //氷
    StageObject_Pond = 106,             //池
    StageObject_Abyss = 107,            //深淵
    StageObject_Flame = 108,            //火炎
    StageObject_Rock = 109,             //石
    StageObject_Key = 110,              //鍵
    StageObject_MagicCircle = 111,      //魔法陣
    
    //その他特殊系は900スタート
    Die = 900                           //死亡時
}
