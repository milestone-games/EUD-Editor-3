﻿Imports System.Windows.Media.Animation

Public Class TEGUIPage
    Private PTEFile As TEFile
    Public ReadOnly Property TEFile As TEFile
        Get
            Return PTEFile
        End Get
    End Property

    Public Function CheckTEFile(tTEfile As TEFile) As Boolean
        Return (tTEfile Is PTEFile)
    End Function

    Public Sub New(tTEFile As TEFile)
        ' 디자이너에서 이 호출이 필요합니다.
        InitializeComponent()

        ' InitializeComponent() 호출 뒤에 초기화 코드를 추가하세요.
        PTEFile = tTEFile

        Script.LoadScript(tTEFile)
        Script.SetTEGUIPage(Me)

        ObjectSelector.SetGUIScriptEditorUI(Script)
        'Script.SetObjectSelecter(ObjectSelector)
    End Sub



    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        AnimationInit()
    End Sub

    Public Sub SaveData()
        Script.Save()
        'MsgBox("세이브 : " & TEFile.FileName)
    End Sub
    Private Sub UserControl_Unloaded(sender As Object, e As RoutedEventArgs)
        Script.Save()
        'MsgBox("세이브 : " & TEFile.FileName)
    End Sub

    Private Sub ObjectSelector_ItemSelect(sender As Object, e As RoutedEventArgs)
        Dim itemtag As String = sender.ToString

        Script.AddItemClick(itemtag)
    End Sub
    Private Sub UserControl_GotFocus(sender As Object, e As RoutedEventArgs)
    End Sub

    Private leftctrldown As Boolean = False
    Private Sub UserControl_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.LeftCtrl
                leftctrldown = True
        End Select
        If leftctrldown Then
            Select Case e.Key
                Case Key.Z
                    'Script.Undo()
                Case Key.R
                    'Script.Redo()
            End Select
        End If
    End Sub

    Private Sub UserControl_PreviewKeyUp(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.LeftCtrl
                leftctrldown = False
        End Select
    End Sub



    Private Sub AnimationInit()
        If True Then
            Dim scale1 As ScaleTransform = New ScaleTransform(1, 1)

            InputDialog.RenderTransformOrigin = New Point(0.5, 0.5)
            InputDialog.RenderTransform = scale1



            Dim myHeightAnimation As DoubleAnimation = New DoubleAnimation()
            myHeightAnimation.From = 0.0
            myHeightAnimation.To = 1.0
            myHeightAnimation.Duration = New Duration(TimeSpan.FromMilliseconds(150))

            Dim myWidthAnimation As DoubleAnimation = New DoubleAnimation()
            myWidthAnimation.From = 0.0
            myWidthAnimation.To = 1.0
            myWidthAnimation.Duration = New Duration(TimeSpan.FromMilliseconds(150))

            Dim myOpacityAnimation As DoubleAnimation = New DoubleAnimation()
            myOpacityAnimation.From = 0.0
            myOpacityAnimation.To = 1.0
            myOpacityAnimation.Duration = New Duration(TimeSpan.FromMilliseconds(150))

            OpenStroyBoard = New Storyboard()
            OpenStroyBoard.Children.Add(myOpacityAnimation)
            OpenStroyBoard.Children.Add(myWidthAnimation)
            OpenStroyBoard.Children.Add(myHeightAnimation)
            Storyboard.SetTargetName(myOpacityAnimation, CreateEditWindow.Name)
            Storyboard.SetTargetName(myWidthAnimation, InputDialog.Name)
            Storyboard.SetTargetName(myHeightAnimation, InputDialog.Name)
            Storyboard.SetTargetProperty(myOpacityAnimation, New PropertyPath(Border.OpacityProperty))
            Storyboard.SetTargetProperty(myHeightAnimation, New PropertyPath("RenderTransform.ScaleY"))
            Storyboard.SetTargetProperty(myWidthAnimation, New PropertyPath("RenderTransform.ScaleX"))
        End If
        If True Then
            Dim scale1 As ScaleTransform = New ScaleTransform(1, 1)

            InputDialog.RenderTransformOrigin = New Point(0.5, 0.5)
            InputDialog.RenderTransform = scale1



            Dim myHeightAnimation As DoubleAnimation = New DoubleAnimation()
            myHeightAnimation.From = 1.0
            myHeightAnimation.To = 0.0
            myHeightAnimation.Duration = New Duration(TimeSpan.FromMilliseconds(150))

            Dim myWidthAnimation As DoubleAnimation = New DoubleAnimation()
            myWidthAnimation.From = 1.0
            myWidthAnimation.To = 0.0
            myWidthAnimation.Duration = New Duration(TimeSpan.FromMilliseconds(150))

            Dim myOpacityAnimation As DoubleAnimation = New DoubleAnimation()
            myOpacityAnimation.From = 1.0
            myOpacityAnimation.To = 0.0
            myOpacityAnimation.Duration = New Duration(TimeSpan.FromMilliseconds(150))

            CloseStroyBoard = New Storyboard()
            CloseStroyBoard.Children.Add(myOpacityAnimation)
            CloseStroyBoard.Children.Add(myWidthAnimation)
            CloseStroyBoard.Children.Add(myHeightAnimation)
            Storyboard.SetTargetName(myOpacityAnimation, CreateEditWindow.Name)
            Storyboard.SetTargetName(myWidthAnimation, InputDialog.Name)
            Storyboard.SetTargetName(myHeightAnimation, InputDialog.Name)
            Storyboard.SetTargetProperty(myOpacityAnimation, New PropertyPath(Border.OpacityProperty))
            Storyboard.SetTargetProperty(myHeightAnimation, New PropertyPath("RenderTransform.ScaleY"))
            Storyboard.SetTargetProperty(myWidthAnimation, New PropertyPath("RenderTransform.ScaleX"))

            AddHandler CloseStroyBoard.Completed, Sub(sender As Object, e As EventArgs)
                                                      CreateEditWindow.Visibility = Visibility.Hidden
                                                  End Sub
        End If



        'InputDialog
    End Sub

    Public Sub OpenEditWindow(ctr As Control, pos As Point)
        ObjectSelector.IsEnabled = False
        Script.IsEnabled = False
        OpenStroyBoard.Begin(Me)
        CreateEditWindow.Visibility = Visibility.Visible
        InputDialog.Content = ctr



        If pos = Nothing Then
            InputDialog.VerticalAlignment = VerticalAlignment.Center
            InputDialog.HorizontalAlignment = HorizontalAlignment.Left
            InputDialog.Margin = New Thickness(ObjectSelector.ActualWidth + 100, 0, 0, 0)
        Else InputDialog.VerticalAlignment = VerticalAlignment.Top
            InputDialog.HorizontalAlignment = HorizontalAlignment.Left


            Dim realh As Integer = Script.ActualHeight
            Dim windowh As Integer = 400 'ctr.Height
            Dim itemh As Integer = 80
            Dim cpos As Integer = pos.Y

            If (cpos + windowh) > realh Then
                InputDialog.VerticalAlignment = VerticalAlignment.Center
                InputDialog.HorizontalAlignment = HorizontalAlignment.Left
                InputDialog.Margin = New Thickness(ObjectSelector.ActualWidth + 100, 0, 0, 0)
            Else
                InputDialog.Margin = New Thickness(ObjectSelector.ActualWidth + pos.X, cpos + itemh, 0, 0)
            End If

        End If
    End Sub



    Public Sub CloseEditWindow()
        ValueSelecter.Child = Nothing
        ObjectSelector.IsEnabled = True
        Script.IsEnabled = True
        CloseStroyBoard.Begin(Me)
    End Sub


    Private OpenStroyBoard As Storyboard
    Private CloseStroyBoard As Storyboard

End Class