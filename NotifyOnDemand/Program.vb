' Copyright Rob Latour, 2022
' https://rlatour.com/notifyondemand

Imports CommunityToolkit.WinUI.Notifications

Module Program

    Sub Main(args As String())

        Dim ReturnCode As Integer = 0

        Try

            Dim CommandLine As String = Environment.CommandLine.ToString.Trim

            'the command line will start off with the program name; as it is of no use, get rid of it

            Dim ProgramName As String = My.Application.Info.AssemblyName.ToUpper

            If CommandLine.ToUpper.Contains(ProgramName) Then
                CommandLine = CommandLine.Remove(0, CommandLine.ToUpper.IndexOf(ProgramName) + ProgramName.Length)
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

                        Const MaxNotificationLinesThatCanBeAdded As Integer = 3

                        Dim NotificationLine As String
                        Dim NotificationLinesAdded As Integer = 0

                        Dim Toast = New ToastContentBuilder()

                        For Each Line In Lines

                            NotificationLine = Line.Trim

                        If NotificationLine.Length > 0 Then

                                Toast.AddText(NotificationLine)

                                NotificationLinesAdded += 1

                                If NotificationLinesAdded = MaxNotificationLinesThatCanBeAdded Then

                                    Exit For

                                End If

                            End If

                        Next

                        If NotificationLinesAdded > 0 Then

                            Dim ToastTime As DateTime = Now

                            With ToastTime
                                Toast.AddCustomTimeStamp(New DateTime(.Year, .Month, .Day, .Hour, .Minute, .Second, DateTimeKind.Local))
                            End With

                        Toast.Show()

                        Threading.Thread.Sleep(100)

                    Else

                            Console.WriteLine("")
                            Console.WriteLine("While there were seperators in the command line, there wasn't any text in between them display in the notification")
                            Console.WriteLine("Notification not created")
                            Console.WriteLine("")
                            ReturnCode = 1

                        End If

                        Toast = Nothing

                Else

                    Console.WriteLine("")
                    Console.WriteLine("While there was a seperator in the command line, there was nothing else to display in the notification")
                    Console.WriteLine("Notification not created")
                    Console.WriteLine("")
                    ReturnCode = 1

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
        Console.WriteLine("NotifyOnDemand Help v1")

        Console.ForegroundColor = OriginalColour
        Console.WriteLine("")
        Console.WriteLine("   Usage:    NotifyOnDemand(.exe) ~ Line1 (~ Line 2) (~ Line 3)")
        Console.WriteLine("")
        Console.WriteLine("   Examples: NotifyOnDemand ~ Backup job is now complete!")
        Console.WriteLine("             NotifyOnDemand ~ Backup job ~ is now complete!")
        Console.WriteLine("             NotifyOnDemand ~ Backup job ~ is now ~ complete!")

        Console.WriteLine("")
        Console.WriteLine("   Note:     the first non-blank character in the command line is used as a line seperator")
        Console.WriteLine("             allowing the notification to display up to three seperate lines")
        Console.WriteLine("")

        If Console.BackgroundColor = ConsoleColor.White Then
            Console.ForegroundColor = ConsoleColor.Blue
        Else
            Console.ForegroundColor = ConsoleColor.White
        End If

        Console.WriteLine("Copyright Rob Latour, 2022")
        Console.WriteLine("https://rlatour.com/notifyondemand")

        Console.ForegroundColor = OriginalColour

    End Sub

End Module