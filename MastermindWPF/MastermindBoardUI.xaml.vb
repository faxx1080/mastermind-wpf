' Author: Frank Migliorino

Public Class MastermindBoardUI

    Private thisColors As Color()
    Private thisPegsMax As Integer
    Private thisCPUChoice As Color()
    Private thisDup As Boolean
    Private won As Boolean

    Property isDone As Boolean
    Property msg As String

    Private thisOutBoxes() As Border
    Private thisInEllipses() As Ellipse

    Property turnNumber As Integer
        Get
            Return windowTurnNumber.Text
        End Get
        Set(value As Integer)
            windowTurnNumber.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Set before clicking a circle.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NextClickColor As Color

    Public Function getCPUChoice() As Color()
        Return thisCPUChoice
    End Function

    Public Sub init(ByVal colorsToUse As Color(), ByVal maxPegs As Integer, cpuChoice() As Color, Optional ByVal dupAllow As Boolean = False)

        thisOutBoxes = {o01, o02, o03, o04, o05, o06, o07,
                                    o08, o09, o10}
        thisInEllipses = {b01, b02, b03, b04, b05, b06,
                                               b07, b08, b09, b10}

        For Each x As Ellipse In thisInEllipses
            Dim thisBoxNum As Integer = CType(x.Tag, Integer)
            If thisBoxNum > maxPegs Then
                x.Visibility = Windows.Visibility.Hidden
            End If
        Next
        For Each x As Border In thisOutBoxes
            Dim thisBoxNum As Integer = CType(x.Tag, Integer)
            If thisBoxNum > maxPegs Then
                x.Visibility = Windows.Visibility.Hidden
            End If
        Next

        thisColors = colorsToUse
        thisPegsMax = maxPegs
        thisDup = dupAllow

        thisCPUChoice = cpuChoice

    End Sub

    Private Sub clickCircle(sender As Object, e As MouseButtonEventArgs) Handles b01.MouseDown, b02.MouseDown, b03.MouseDown,
                                                                                 b04.MouseDown, b05.MouseDown, b06.MouseDown,
                                                                                 b07.MouseDown, b08.MouseDown, b09.MouseDown, b10.MouseDown
        If Not isDone And e.LeftButton = MouseButtonState.Pressed Then
            If Not (thisDup) Then
                For Each y As Ellipse In thisInEllipses
                    If CType(y.Fill, SolidColorBrush).Color = NextClickColor Then
                        'RaiseEvent output("Duplicate color drop attempted, not allowed.")
                        Dim other As Color = CType(y.Fill, SolidColorBrush).Color, clicked As Color = CType(CType(sender, Ellipse).Fill, SolidColorBrush).Color
                        y.Fill = New SolidColorBrush(clicked)
                        CType(sender, Ellipse).Fill = New SolidColorBrush(other)
                        Return
                    End If
                Next
            End If
            CType(sender, Ellipse).Fill = New SolidColorBrush(NextClickColor)
        End If
    End Sub

    Public Sub checkRowPublic()
        Dim inC As New ArrayList, outBlocks As New ArrayList
        'For Each x As System.Windows.UIElement In CType(Me.Content, Grid).Children
        '    If TypeOf (x) Is Ellipse And x.Visibility = Windows.Visibility.Visible Then
        '        inC.Add(CType(CType(x, Ellipse).Fill, SolidColorBrush).Color)
        '    End If
        'Next
        For Each x In thisInEllipses
            If x.Visibility = Windows.Visibility.Visible Then
                inC.Add(CType(x.Fill, SolidColorBrush).Color)
            End If
        Next
        Dim outC As Color() = checkColorInput(inC.ToArray(GetType(Color)))
        For x As Integer = outC.GetLowerBound(0) To outC.GetUpperBound(0)
            thisOutBoxes(x).Background = New SolidColorBrush(outC(x))
        Next
    End Sub

    'Io = RGBB
    'W =  BGRY
    'O =  BGWW
    ''' <summary>
    ''' input colors, outputs match as color array. White = nope, Gray = color Not pos, Black = all
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function checkColorInput(ByVal inColors() As Color) As Color()
        Dim x As Integer
        Dim colOut(inColors.Length - 1) As Color
        Dim tempIn As ArrayList = New ArrayList(inColors)
        Dim tempCPU As New ArrayList(thisCPUChoice)
        For i As Integer = 0 To tempIn.Count - 1
            If i < tempIn.Count AndAlso tempIn(i) = tempCPU(i) Then
                Try
                    colOut(x) = Colors.Black
                    x += 1
                    tempIn.RemoveAt(i)
                    tempCPU.RemoveAt(i)
                    i -= 1
                Catch ex As Exception
                    Exit For
                End Try
            ElseIf i >= tempIn.Count Then
                Exit For
            End If
        Next
        If tempCPU.Count = 0 Then
            msg = "Winner!"
            Dim y As New System.Media.SoundPlayer(My.Resources.win)
            y.Play()
            Return colOut 'WINNER
        End If

        For i As Integer = 0 To tempIn.Count - 1
            Dim thisNum As Integer
            Try
                thisNum = Array.IndexOf(tempCPU.ToArray, tempIn(i))
            Catch ex As Exception
                Exit For
            End Try
            If thisNum >= 0 Then
                tempCPU.RemoveAt(thisNum)
                tempIn.RemoveAt(i)
                i -= 1
                colOut(x) = Colors.Gray
                x += 1
            End If
        Next
        For i As Integer = x To colOut.Length - 1
            colOut(i) = Colors.White
        Next
        Return colOut
    End Function

    'Good code to know:
    'For Each x As System.Windows.UIElement In CType(Me.Content, Grid).Children
    '    If TypeOf (x) Is Ellipse Then
    '        Dim thisBoxNum As Integer = CType(CType(x, Ellipse).Tag, Integer)
    '        If thisBoxNum > maxPegs Then
    '            CType(x, Ellipse).Visibility = Windows.Visibility.Hidden
    '        End If
    '    End If
    '    If TypeOf (x) Is Border Then
    '        Dim thisBoxNum As Integer = CType(CType(x, Border).Tag, Integer)
    '        If thisBoxNum > maxPegs Then
    '            CType(x, Border).Visibility = Windows.Visibility.Hidden
    '        End If
    '    End If
    'Next

End Class