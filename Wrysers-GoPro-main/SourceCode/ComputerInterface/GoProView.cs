using UnityEngine;
using System;
using ComputerInterface;
using ComputerInterface.ViewLib;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;

namespace WrysersGoPro
{
    class GoProView : ComputerView
    {
        // This is called when you view is opened
        const string titleColour = "66ff00";
        const string subsettingsColour = "ffffff";
        public static GoProView instance;
        private readonly UISelectionHandler selectionHandler;

        public GoProView()
        {
            instance = this;

            selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);

            selectionHandler.MaxIdx = 1;

            selectionHandler.OnSelected += OnEntrySelected;

            selectionHandler.ConfigureSelectionIndicator($"<color=#{subsettingsColour}>></color> ", "", "  ", "");
        }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            UpdateScreen();
        }
        public void UpdateScreen()
        {
            SetText(str =>
            {
                str.BeginCenter();
                str.MakeBar('-', SCREEN_WIDTH, 0, "ffffff10");
                str.AppendClr("Wryser's GoPro!", titleColour).EndColor().AppendLine();
                str.AppendLine("By Wryser");
                str.MakeBar('-', SCREEN_WIDTH, 0, "ffffff10");
                str.EndAlign().AppendLines(1);
                str.MakeBar(' ', SCREEN_WIDTH, 0, "ffffff10");
                str.AppendLine(selectionHandler.GetIndicatedText(0, $"<color={(GoProManager.FOVislock ? string.Format("#{0}>[FOVLock Enabled]", titleColour) : "red>[FOVLock Disabled]")}</color>"));
                str.AppendLine(selectionHandler.GetIndicatedText(1, $"<color={(GoProManager.Controlsarelocked ? string.Format("#{0}>[ControlLock Enabled]", titleColour) : "red>[ControlLock Disabled]")}</color>"));
                str.AppendLines(1);
                str.MakeBar(' ', SCREEN_WIDTH, 0, "ffffff10");
            });
        }
        private void OnEntrySelected(int index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        GoProManager.FOVislock = !GoProManager.FOVislock;
                        GoProConfig.FOVLock = GoProManager.FOVislock;
                        UpdateScreen();
                        break;
                    case 1:
                        GoProManager.Controlsarelocked = !GoProManager.Controlsarelocked;
                        GoProConfig.ControlLock = GoProManager.Controlsarelocked;
                        UpdateScreen();
                        break;
                }
            }
            catch (Exception e) { Debug.Log(e.ToString()); }
        }

        // you can do something on keypresses by overriding "OnKeyPressed"
        // it get's an EKeyboardKey passed as a parameter which wraps the old character string
        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (selectionHandler.HandleKeypress(key))
            {
                UpdateScreen();
                return;
            }
            switch (key)
            {
                case EKeyboardKey.Back:
                    // "ReturnToMainMenu" will basically switch to the main menu again
                    ReturnToMainMenu();
                    break;
                case EKeyboardKey.Option1:
                    // If you want to switch to another view you can do it like this
                    break;
                case EKeyboardKey.Up:
                    selectionHandler.MoveSelectionUp();
                    UpdateScreen();
                    break;
                case EKeyboardKey.Down:
                    selectionHandler.MoveSelectionDown();
                    UpdateScreen();
                    break;
            }
        }
    }
}