﻿using PicView.Animations;
using System.Windows.Controls;
using static PicView.Animations.MouseOverAnimations;

namespace PicView.Views.UserControls
{
    public partial class RotateLeftButton : UserControl
    {
        public RotateLeftButton()
        {
            InitializeComponent();

            Loaded += delegate
            {
                TheButton.PreviewMouseLeftButtonDown += delegate
                {
                    ButtonMouseOverAnim(IconBrush, false, true);
                    ButtonMouseOverAnim(TheButtonBrush, false, true);
                    AnimationHelper.MouseEnterBgTexColor(TheButtonBrush);
                };

                TheButton.MouseEnter += delegate
                {
                    ButtonMouseOverAnim(IconBrush);
                    AnimationHelper.MouseEnterBgTexColor(TheButtonBrush);
                };

                TheButton.MouseLeave += delegate
                {
                    ButtonMouseLeaveAnim(IconBrush);
                    AnimationHelper.MouseLeaveBgTexColor(TheButtonBrush);
                };

                TheButton.Click += async (_, _) => await UILogic.TransformImage.Rotation.RotateAndMoveCursor(false, TheButton).ConfigureAwait(false);
            };
        }
    }
}