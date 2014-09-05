// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.

using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    public partial class ConfigureGeneralSettings
    {
        int nextTabIndex = 1;
        int nextRowPosition = 14;
        List<Action> OnSave = new List<Action>();

        void AddBlankRow()
        {
            nextRowPosition += 20;
        }

        void AddPasswordField(string caption, string description, Func<Settings, string> getter, Action<Settings, string> setter)
        {
            var textBox = AddStringField(caption, description, getter, setter);
            textBox.PasswordChar = '*';
        }

        TextBox AddStringField(string caption, string description, Func<Settings, string> getter, Action<Settings, string> setter)
        {
            return AddField(caption, description, getter(Settings.Default), tb => setter(Settings.Default, tb.Text));
        }

        void AddStringField(string caption, string description, string setting)
        {
            AddField(caption, description, Settings.Default[setting].ToString(), tb => Settings.Default[setting] = tb.Text);
        }

        void AddKeyPressField(string caption, string description)
        {
            var textBox = AddField(caption, description, "ALT+F9", tb => { });
            textBox.ReadOnly = true;
        }

        void AddNumberField(string caption, string description, string setting)
        {
            AddField(caption, description, Settings.Default[setting].ToString(), tb =>
            {
                var number = 0.0d;
                if (double.TryParse(tb.Text, out number))
                    Settings.Default[setting] = number;
            });
        }

        void AddTimeField(string caption, string description, string setting)
        {
            panel.Controls.Add(new Label
            {
                Location = new System.Drawing.Point(440, nextRowPosition),
                Size = new System.Drawing.Size(60, 16),
                TabIndex = nextTabIndex++,
                Text = "seconds"
            });

            AddField(caption, description, ((TimeSpan)Settings.Default[setting]).TotalSeconds.ToString(), tb =>
            {
                var newSeconds = 0.0;
                if (double.TryParse(tb.Text, out newSeconds))
                    Settings.Default[setting] = newSeconds.Seconds();
            });
        }

        TextBox AddField(string caption, string description, string value, Action<TextBox> onSave)
        {
            panel.Controls.Add(new Label
            {
                Location = new System.Drawing.Point(14, nextRowPosition),
                Size = new System.Drawing.Size(269, 16),
                TabIndex = nextTabIndex++,
                Text = caption
            });

            var textBox = new TextBox
            {
                Location = new System.Drawing.Point(301, nextRowPosition - 3),
                Size = new System.Drawing.Size(134, 22),
                TabIndex = nextTabIndex++,
                Tag = description,
                Text = value
            };
            textBox.Enter += new System.EventHandler(this.OnFocus);
            panel.Controls.Add(textBox);

            OnSave.Add(() => onSave(textBox));
            nextRowPosition += 28;

            return textBox;
        }
    }
}
