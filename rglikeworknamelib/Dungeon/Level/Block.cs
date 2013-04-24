using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class Block {
        public int id;
        public Color lightness;
        public bool explored;

        public static string GetSmartActionName(SmartAction smartAction)
        {
            switch (smartAction) {
                case SmartAction.ActionOpenContainer:
                    return "��������� ����������";
                case SmartAction.ActionOpenClose:
                    return "�������/�������";
                default:
                    return "���������";
            }
        }
    }
}