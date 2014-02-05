Class MainWindow

    Public IsDuplicatesAllowed As Boolean
    Public currentColors As Color()
    Property maxColors As Integer
    Property maxPegs As Integer

    Private boards() As MastermindBoardUI

    Private Sub btnNewGame_Click(sender As Object, e As RoutedEventArgs) Handles btnNewGame.Click
        If colorsSlider.Value < pegsSlider.Value + 2 Then
            Beep()
            stbLabel.Content = "Must have 2 more colors than pegs."
            Return
        End If
        If sender.Content = "Quit & New Game" Then
            'Weird Problem - XP Cannot support this code section, so it crashes here on XP only.
            'I had no XP computer to test it on. It even worked on the Seaford Public Library's Windows 7 Comp.
            'Quick and dirty reset - let the .NET garbage collector clear everything for us
            Dim x As New MainWindow
            x.WindowStartupLocation = Windows.WindowStartupLocation.Manual
            x.Left = Me.Left
            x.Top = Me.Top
            x.Width = Me.Width
            x.Height = Me.Height
            Me.Close()
            x.Show()
        End If
        sender.Content = "Quit & New Game"

        stbLabel.Content = "Guess the computer's color choices and order."
        pegsSlider.IsEnabled = False
        colorsSlider.IsEnabled = False
        chkDup.IsEnabled = False
        maxColors = colorsSlider.Value
        maxPegs = pegsSlider.Value
        currentColors = allColors.Take(maxColors).ToArray
        'Enable Color Well
        wellLbl.Visibility = Windows.Visibility.Visible
        wellLbl.UpdateLayout()

        well01.Visibility = Windows.Visibility.Visible
        well02.Visibility = Windows.Visibility.Visible
        well03.Visibility = Windows.Visibility.Visible
        well04.Visibility = Windows.Visibility.Visible
        well05.Visibility = Windows.Visibility.Visible
        well06.Visibility = Windows.Visibility.Visible
        If maxColors > 6 Then
            well07.Visibility = Windows.Visibility.Visible
            If maxColors > 7 Then
                well08.Visibility = Windows.Visibility.Visible
                If maxColors > 8 Then
                    well09.Visibility = Windows.Visibility.Visible
                    If maxColors > 9 Then
                        well10.Visibility = Windows.Visibility.Visible
                        If maxColors > 10 Then
                            well11.Visibility = Windows.Visibility.Visible
                            If maxColors > 11 Then
                                well12.Visibility = Windows.Visibility.Visible
                            End If
                        End If
                    End If
                End If
            End If
        End If

        boards = {board01, board02, board03,
                  board04, board05, board06,
                  board07, board08, board09,
                  board10}

        'Start
        currentBoard = 0
        initBoard(currentBoard)
    End Sub
    Private thisCPUChoice As Color()
    Private Sub initBoard(boardNum As Integer)
        btnCheck.IsEnabled = True
        boards(boardNum).Visibility = Windows.Visibility.Visible

        If thisCPUChoice Is Nothing Then
            thisCPUChoice = Array.CreateInstance(GetType(Color), maxPegs)
            Dim rng As New Random
            If Not chkDup.IsChecked Then
                Dim rnums(maxPegs) As Integer
                For i As Integer = 0 To maxPegs - 1
                    Dim x As Integer = rng.Next(0, currentColors.GetUpperBound(0))
                    If rnums.Contains(x) Then
                        i = i - 1 'Try again
                    Else
                        rnums(i) = x
                    End If
                Next
                For i As Integer = thisCPUChoice.GetLowerBound(0) To thisCPUChoice.GetUpperBound(0)
                    thisCPUChoice(i) = currentColors(rnums(i))
                Next
            Else
                Dim rnums(maxPegs) As Integer
                For i As Integer = 0 To maxPegs - 1
                    Dim x As Integer = rng.Next(0, currentColors.GetUpperBound(0) - 1)
                    rnums(i) = x
                Next
                For i As Integer = thisCPUChoice.GetLowerBound(0) To thisCPUChoice.GetUpperBound(0)
                    thisCPUChoice(i) = currentColors(rnums(i))
                Next
            End If
#If CONFIG = "Debug" Then
            For Each xy As System.Windows.UIElement In CType(Me.Content, Grid).Children
                If xy.GetType Is GetType(Rectangle) Then
                    Try
                        CType(xy, Rectangle).Fill = New SolidColorBrush(thisCPUChoice(CType(xy, Rectangle).Tag))
                    Catch ex As Exception
                        Exit For
                    End Try
                End If
            Next
#End If
        End If

        boards(boardNum).init(currentColors, maxPegs, thisCPUChoice, chkDup.IsChecked)
    End Sub

    ''' <summary>
    ''' Zero-Based.
    ''' </summary>
    ''' <remarks></remarks>
    Private currentBoard As Integer
    Private _lastClickedWell As Border

    Private Sub well_click(sender As Object, e As MouseEventArgs) Handles well01.MouseDown,
        well02.MouseDown, well03.MouseDown, well04.MouseDown, well05.MouseDown, well06.MouseDown,
        well07.MouseDown, well08.MouseDown, well09.MouseDown, well10.MouseDown, well11.MouseDown, well12.MouseDown
        If Not sender.GetType Is GetType(Border) Then
            Return 'Stupidity check
        End If
        If Not _lastClickedWell Is Nothing Then
            _lastClickedWell.BorderThickness = New Thickness(1)
        End If
        If e.LeftButton = MouseButtonState.Pressed Then
            boards(currentBoard).NextClickColor = CType(CType(sender, Border).Background, SolidColorBrush).Color
            CType(sender, Border).BorderThickness = New Thickness(4)
            'CType(sender, Border).BorderBrush = Brushes
        Else
            CType(sender, Border).BorderThickness = New Thickness(1)
        End If
        _lastClickedWell = sender
    End Sub

    Private Sub CheckRowClick(sender As Object, e As RoutedEventArgs)
        boards(currentBoard).checkRowPublic()
        Select Case boards(currentBoard).msg
            Case "Winner!"
                stbLabel.Content = "Winner!"
                boards(currentBoard).isDone = True
                btnCheck.IsEnabled = False
                Return
            Case Else
                stbLabel.Content = boards(currentBoard).msg
        End Select
        boards(currentBoard).isDone = True
        If currentBoard >= 9 Then
            boards(currentBoard - 1).isDone = True
            Dim y As Integer = 0
            stbLabel.Content = "Loser. Answer: "
            For Each x As UIElement In stBar.Items
                If x.GetType() = GetType(Rectangle) Then
                    Try
                        CType(x, Rectangle).Fill = New SolidColorBrush(thisCPUChoice(y))
                    Catch ex As Exception
                        Exit For
                    End Try
                    CType(x, Rectangle).Stroke = New SolidColorBrush(Colors.Black)
                    y += 1
                End If
            Next
            Beep()
            btnCheck.IsEnabled = False
            Dim cpuChoice() As Color = boards(currentBoard).getCPUChoice()
        Else
            currentBoard += 1
            initBoard(currentBoard)
        End If
    End Sub

    'Private WithEvents timer As New System.Windows.Threading.DispatcherTimer
    'Private Sub output(msg As String)
    '    Beep()
    '    stbLabel.Content = msg
    '    timer.Interval = New TimeSpan(0, 0, 3)
    '    timer.Start()
    'End Sub

    'Private Sub timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick
    '    stbLabel.Content = ""
    '    timer.IsEnabled = False
    'End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Dim x As New AboutDialog
        x.ShowDialog()
    End Sub
End Class