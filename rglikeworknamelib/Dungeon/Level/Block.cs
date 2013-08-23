using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Block {
        public string Id;
        public Color Lightness;
        public bool Explored;

        public string Mtex = "0";

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

        public void SetLight(Color color) {
            Lightness = color;
        }
    }
}