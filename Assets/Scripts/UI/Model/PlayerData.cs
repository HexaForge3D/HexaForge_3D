public class PlayerData
{
    public string Id {  get; private set; }
    public string Name { get; private set; }
    public string Job {  get; private set; }
    public int Hp {  get; private set; }
    public int Mp { get; private set; }
    public int Atk { get; private set; }
    public int Def { get; private set; }

    public PlayerData(string id, string name, string job, int hp, int mp, int atk, int def)
    {
        Id = id;
        Name = name;
        Job = job;
        Hp = hp;
        Mp = mp;
        Atk = atk;
        Def = def;
    }

}
