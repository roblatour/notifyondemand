' Copyright Rob Latour, 2022 - 2024
' https://rlatour.com/notifyondemand

Imports System.Runtime.InteropServices
Imports CommunityToolkit.WinUI.Notifications

Module Program
    Sub Main(args As String())

        Dim ReturnCode As Integer = 0

        Try

            Dim CommandLine As String

#If DEBUG Then
            CommandLine = "~ this is a test ~ of a three line ~ notification"
#Else
            CommandLine = Environment.CommandLine.ToString.Trim
#End If

            Dim ProgramName As String = My.Application.Info.AssemblyName

            ' the first part of the command line contains the program name; this needs to be removed 
            ' complicating things, the program name may or may not
            '   include a directory path,
            '   be surrounded with quotes, and/or
            '   have ".exe" at its end

            If CommandLine.StartsWith("""") AndAlso CommandLine.ToUpper.Contains(ProgramName.ToUpper) Then

                CommandLine = CommandLine.Remove(0, 1) ' remove the first quote

                Dim IndexOfSecondQuote As Integer = CommandLine.IndexOf("""")

                If IndexOfSecondQuote > 1 Then

                    ' remove up to and including the second quote
                    CommandLine = CommandLine.Remove(0, IndexOfSecondQuote + 1)

                Else

                    ' there is no second quote, so remove up to and including the program name
                    CommandLine = CommandLine.Remove(0, CommandLine.IndexOf(ProgramName) + ProgramName.Length)

                    If CommandLine.ToUpper.StartsWith(".EXE") Then CommandLine = CommandLine.Remove(0, 4)

                End If

            ElseIf CommandLine.ToUpper.Contains(ProgramName.ToUpper) Then

                CommandLine = CommandLine.Remove(0, CommandLine.ToUpper.IndexOf(ProgramName.ToUpper) + ProgramName.Length)

                If CommandLine.ToUpper.StartsWith(".EXE") Then CommandLine = CommandLine.Remove(0, 4)

            End If

            CommandLine = CommandLine.Trim

            If CommandLine.Length = 0 Then

                DisplayHelp()

            Else


                ' seperate the command line into up to three lines, with each line being added into the notification
                ' if more than three lines are found, ignore the extra lines

                If CommandLine.Length > 1 Then

                    ' determine the seperating charactor

                    Dim Seperator As String = CommandLine.Remove(1)

                    'remove the sperator indicator from the command line
                    CommandLine = CommandLine.Remove(0, 1).Trim

                    Dim Lines() As String = CommandLine.Split(Seperator)

                    Const MaxParameters As Integer = 4
                    Const MaxNotificationLines As Integer = MaxParameters - 1

                    Dim LinesProcessed As Integer = 0
                    Dim NotificationLinesProcessed As Integer = 0
                    Dim TooManyNotificationLines As Boolean = False

                    Dim WorkingLine As String

                    Dim WebSite As String = String.Empty

                    Dim Toast = New ToastContentBuilder()

                    For Each Line In Lines

                        WorkingLine = Line.Trim

                        If WorkingLine.Length > 0 Then

                            If WorkingLine.ToUpper.StartsWith("HTTP") Then
                                WebSite = WorkingLine
                            Else
                                If (NotificationLinesProcessed < MaxNotificationLines) Then
                                    Toast.AddText(WorkingLine)
                                    NotificationLinesProcessed += 1
                                Else
                                    TooManyNotificationLines = True
                                    Exit For
                                End If

                            End If

                            LinesProcessed += 1

                            If (LinesProcessed > MaxParameters) Then

                                Exit For

                            End If

                        End If

                    Next

                    If TooManyNotificationLines Then

                        Console.WriteLine("")
                        Console.WriteLine("There were too many lines to display in the notification")
                        Console.WriteLine("Notification not created")
                        Console.WriteLine("")
                        ReturnCode = 1

                    ElseIf (NotificationLinesProcessed = 0) Then

                        Console.WriteLine("")
                        Console.WriteLine("There wasn't any text to display in the notification")
                        Console.WriteLine("Notification not created")
                        Console.WriteLine("")
                        ReturnCode = 1

                    Else

                        Dim ToastTime As DateTime = Now

                        With ToastTime
                            Toast.AddCustomTimeStamp(New DateTime(.Year, .Month, .Day, .Hour, .Minute, .Second, DateTimeKind.Local))
                        End With

                        'the following suppresses the notification being added to the Windows Notification Center if the user clicks on the toast
                        Toast.SetProtocolActivation(New Uri(My.Application.Info.DirectoryPath & "\dummy.txt"))

                        If WebSite > String.Empty Then
                            Toast.SetProtocolActivation(New Uri(WebSite))
                        End If

                        Toast.Show()

                        Threading.Thread.Sleep(100)

                    End If


                    Toast = Nothing

                End If

            End If

        Catch ex As Exception

            Console.WriteLine("")
            Console.WriteLine(ex.Message)
            Console.WriteLine("")
            ReturnCode = 1

        End Try

        Environment.Exit(ReturnCode)

    End Sub

    Private Sub DisplayHelp()

        Dim OriginalColour As ConsoleColor = Console.ForegroundColor

        If Console.BackgroundColor = ConsoleColor.White Then
            Console.ForegroundColor = ConsoleColor.Blue
        Else
            Console.ForegroundColor = ConsoleColor.White
        End If

        Console.WriteLine("")
        Console.WriteLine("NotifyOnDemand Help v1.2")

        Console.ForegroundColor = OriginalColour
        Console.WriteLine("")
        Console.WriteLine("   Usage:    NotifyOnDemand(.exe) ~ Line1 (~ Line 2) (~ Line 3) (~website)")
        Console.WriteLine("")
        Console.WriteLine("   Examples: NotifyOnDemand ~ Backup job is now complete!")
        Console.WriteLine("             NotifyOnDemand ~ Backup job ~ is now complete!")
        Console.WriteLine("             NotifyOnDemand ~ Backup job ~ is now ~ complete!")
        Console.WriteLine("             NotifyOnDemand ~ Click here ~ to open the Google search site ~ https:\\google.com")
        Console.WriteLine("             NotifyOnDemand ~ Click here ~ to open the Google search site ~ in your default browser ~ https:\\google.com")

        Console.WriteLine("")
        Console.WriteLine("   Notes:    the first non-blank character in the command line is used as a line seperator")
        Console.WriteLine("             the notification may display up to three seperate lines")
        Console.WriteLine("             an optional webSite may be added as a final parameter")
        Console.WriteLine("             if used, the website must start with http")
        Console.WriteLine("             if the user clicks on the notification, the website will be opened in the default browser")
        Console.WriteLine("             if the user clicks on the notification's close button ('X') the website will not be opened")

        Console.WriteLine("")

        If Console.BackgroundColor = ConsoleColor.White Then
            Console.ForegroundColor = ConsoleColor.Blue
        Else
            Console.ForegroundColor = ConsoleColor.White
        End If

        Console.WriteLine("Copyright Rob Latour, 2022 - 2024")
        Console.WriteLine("https://rlatour.com/notifyondemand")

        Console.ForegroundColor = OriginalColour

    End Sub

End Module
