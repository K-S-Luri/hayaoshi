﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hayaoshi
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    /// 
    public partial class Hayaoshi : Window
    {
        bool pushed = false;
        Key[] judgingButton;
        Player[] players;
        int playerSize = 4;
        Player pushPlayer;
        List<(Player player, JudgeStatus judge)> history = new List<(Player player, JudgeStatus judge)>();
        int questionNumber = 0;
        Player throughPlayer;
        Dictionary<string, string> sounds = new Dictionary<string, string>() {
            {"button", "C:\\Users\\Tsurusaki\\Git\\hayaoshi\\buttonSound6.wav"},
            {"correct", "C:\\Users\\Tsurusaki\\Git\\hayaoshi\\correctSound6.wav"},
            {"wrong", "C:\\Users\\Tsurusaki\\Git\\hayaoshi\\wrongBuzzer2.wav" },
            {"buzzer", "C:\\Users\\Tsurusaki\\Git\\hayaoshi\\buzzer.wav" }
        };
        string[] questionSounds =  {
            "C:\\Users\\Tsurusaki\\Git\\hayaoshi\\開演ブザー.wav",
            "C:\\Users\\Tsurusaki\\Git\\hayaoshi\\クイズ1.m4a",
            "C:\\Users\\Tsurusaki\\Git\\hayaoshi\\クイズ2.m4a",
            "C:\\Users\\Tsurusaki\\Git\\hayaoshi\\クイズ3.m4a"
        };

        public Hayaoshi()
        {
            InitializeComponent();
            string[] names = new string[4] { "ジェフ・ベゾス", "ビル・ゲイツ", "Warren Buffett", "孫 正義" };
            Label[] nameLabels = new Label[4] { name0, name1, name2, name3 };
            Label[] keyLabels = new Label[4] { key0, key1, key2, key3 };
            Label[] lightlabels = new Label[4] { push0, push1, push2, push3 };
            Key[] buttons = new Key[4] { Key.D2, Key.T, Key.K, Key.OemBackslash };
            Label[] pointLabels = new Label[4] { point0, point1, point2, point3 };
            Label[] missLabels = new Label[4] { miss0, miss1, miss2, miss3 };
            players = new Player[playerSize];
            for (int i = 0; i < playerSize; i++)
            {
                players[i] = new Player();
                players[i].Name = names[i];
                players[i].NameLabel = nameLabels[i];
                players[i].KeyLabel = keyLabels[i];
                players[i].LightLabel = lightlabels[i];
                players[i].Point = 0;
                players[i].Mistake = 0;
                players[i].PointLabel = pointLabels[i];
                players[i].MistakeLabel = missLabels[i];
                players[i].Button = buttons[i];
            }

            push0.Content = "aa";
            pushed = false;
            judgingButton = new Key[] {Key.O, Key.X};
            
            for (int i = 0; i < playerSize; i++)
            {
                players[i].NameLabel.Content = players[i].Name;
                players[i].LightLabel.Content = "";
                players[i].PointLabel.Content = players[i].Point.ToString();
                players[i].MistakeLabel.Content = players[i].Mistake.ToString();
            }

            questionNumberLabel.Content = (questionNumber + 1).ToString() + "問目";
        }

        private void KeyPush(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < playerSize; i++)
            {
                if (e.Key == players[i].Button)
                {
                    if (!pushed)
                    {
                        players[i].LightLabel.Content = "!";
                        pushed = true;
                        through.Content = "無効";
                        pushPlayer = players[i];
                        playSound(sounds["button"]);
                        message.Content = "正解ならばoを、不正解ならばxを押してください";
                    }
                }
            }
            if (judgingButton.Contains(e.Key))
            {
                if (pushed)
                {
                    pushed = false;
                    pushPlayer.LightLabel.Content = "";
                    if (e.Key == Key.O)
                    {
                        playSound(sounds["correct"]);
                        pushPlayer.Point++;
                        pushPlayer.PointLabel.Content = pushPlayer.Point.ToString();
                        history.Add((pushPlayer, JudgeStatus.Point));
                        questionNumber++;
                        questionNumberLabel.Content = (questionNumber + 1).ToString() + "問目";
                    } else
                    {
                        playSound(sounds["wrong"]);
                        pushPlayer.Mistake++;
                        pushPlayer.MistakeLabel.Content = pushPlayer.Mistake.ToString();
                        history.Add((pushPlayer, JudgeStatus.Mistake));
                        questionNumber++;
                        questionNumberLabel.Content = (questionNumber + 1).ToString() + "問目";
                    }
                    message.Content = "出題中";
                    through.Content = "through";
                }
            }
        }

        private void UndoClick(object sender, RoutedEventArgs e)
        {
            if (!pushed)
            {
                if (history.Count > 0)
                {
                    (Player player, JudgeStatus judge) latest = history[history.Count - 1];
                    history.RemoveAt(history.Count - 1);
                    if (latest.judge == JudgeStatus.Point)
                    {
                        latest.player.Point--;
                        latest.player.PointLabel.Content = latest.player.Point.ToString();
                    } else if (latest.judge == JudgeStatus.Mistake)
                    {
                        latest.player.Mistake--;
                        latest.player.PointLabel.Content = latest.player.Point.ToString();
                    } else if (latest.judge == JudgeStatus.Through)
                    {
                        
                    } else if (latest.judge == JudgeStatus.Invalid)
                    {

                    }
                    stopSound();
                    questionNumber--;
                    questionNumberLabel.Content = (questionNumber + 1).ToString() + "問目";
                }
            }
        }

        private void ThroughClick(object sender, RoutedEventArgs e)
        {
            if (!pushed)
            {
                history.Add((throughPlayer, JudgeStatus.Through));
            } else
            {
                history.Add((throughPlayer, JudgeStatus.Invalid));
                message.Content = "出題中";
                through.Content = "through";
            }
            stopSound();
            questionNumber++;
            questionNumberLabel.Content = (questionNumber + 1).ToString() + "問目";
            pushed = false;
            pushPlayer.LightLabel.Content = "";
        }

        private void questionClick(object sender, RoutedEventArgs e) {
            if (!pushed) {
                stopSound();
                int len = questionSounds.Length;
                if (questionNumber < len) {
                    playSound(questionSounds[questionNumber]);
                } else {
                    message.Content = "もう問題がありません";
                }
            }
        }

        //private static string KeyEventArgsToString(KeyEventArgs e)
        //{
        //    Key key = e.Key;
        //    Key systemKey = e.SystemKey;
        //    KeyStates keyStates = e.KeyStates;
        //    bool isRepeat = e.IsRepeat;
        //    string s = "";
        //    s += string.Format("{0}", key);      
        //    //ModifierKeys modifierKeys = Keyboard.Modifiers;
        //    //if ((modifierKeys & ModifierKeys.Alt) != ModifierKeys.None)
        //    //    s += "  Alt ";
        //    //if ((modifierKeys & ModifierKeys.Control) != ModifierKeys.None)
        //    //    s += "  Control ";
        //    //if ((modifierKeys & ModifierKeys.Shift) != ModifierKeys.None)
        //    //    s += "  Shift ";
        //    //if ((modifierKeys & ModifierKeys.Windows) != ModifierKeys.None)
        //    //    s += "  Windows";
        //    //if (key == Key.System)
        //    //    s += systemKey;
        //    return s;
        //}

        private void GridLoaded(object sender, RoutedEventArgs e)
        {
            grid.Focus();
        }

        private void playSound(string path) {
            stopSound();
            Microsoft.SmallBasic.Library.Sound.Play(path);
        }

        private void stopSound() {
            foreach (string item in sounds.Values) {
                Microsoft.SmallBasic.Library.Sound.Stop(item);
            }
            foreach (string item in questionSounds) {
                Microsoft.SmallBasic.Library.Sound.Stop(item);
            }
        }
    }
}
