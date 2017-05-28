Imports System.IO

Public Class Form1
    Dim filelist() As String
    Dim recursivelist As List(Of String)
    Dim fpath As String
    Dim foundFile As String = ""
    Dim filename As String
    Dim extension As String
    Dim isprefix As Boolean = False
    Dim isnewname As Boolean = False
    Dim prefix As String
    Dim newname As String
    Dim iskana As String
    Dim najdena As Boolean





    Private Sub OpenFileDialog1_FileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        Dim strm As System.IO.Stream


        strm = OpenFileDialog1.OpenFile() 'Odpres datoteko


        TextBox1.Text = ""


        If Not (strm Is Nothing) Then


            'insert code to read the file data

            fpath = OpenFileDialog1.FileName
            ListBox1.Text = fpath

            Dim fi As New FileInfo(fpath) 'pridobi objekt za informacije o datoteki/mapi
            Dim folder As String
            folder = fi.Directory.ToString
            ListBox1.Items.Add("Izbrana mapa je: " + folder)


            filelist = System.IO.Directory.GetFiles(folder)


            For Each foundFile As String In filelist
                ListBox1.Items.Add(foundFile)
            Next

        End If

        strm.Close() 'Zapres datoteko

    End Sub


    Private Function IskanjeDatoteke(ByVal initial As String) As List(Of String)
        'povzeto po: https://www.dotnetperls.com/recursive-file-directory-vbnet
        ' Seznam shrani rezultate iskanja
        Dim rezultat As New List(Of String)

        ' Sklad hrani trenutno mapo in njene podmape
        Dim sklad As New Stack(Of String)

        ' Dodajanje trenutne mape za izbiranje mape je potrebno klikniti datoteko v njej...
        'Moynost izboljsave
        sklad.Push(initial)

        ' Sprehod čez mape v skladu
        Do While (sklad.Count > 0)
            ' Vrni ime vrhnje mape kot string
            Dim dir As String = sklad.Pop
            Try
                ' dodajanje vseh najdenih datotek v seznam
                rezultat.AddRange(Directory.GetFiles(dir, "*.*"))

                ' sprehod čez podmape in dodanjanje njihov podmap v sklad
                Dim directoryName As String
                For Each directoryName In Directory.GetDirectories(dir)
                    sklad.Push(directoryName)
                Next

            Catch ex As Exception
                ListBox1.Items.Add(ex.ToString)
            End Try
        Loop

        ' Vrnemo seznam najdenih datotek
        Return rezultat
    End Function

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        OpenFileDialog1.Title = "Please Select a File"


        OpenFileDialog1.InitialDirectory = "C:\"


        OpenFileDialog1.ShowDialog()
        Me.Button3.Enabled = True
        Me.Button4.Enabled = True
        Me.TextBox1.Enabled = True
        Me.btnFind.Enabled = True
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        isprefix = True
        isnewname = False 'pogoj ki ga je potrebno dodati za več krogov izvedbe, bug pri prikazu na predavanjih
        ListBox1.Items.Clear()
        ListBox1.Items.Add("Vnesi besedilo za prefix.")
        Me.Button2.Enabled = True

    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        isnewname = True
        isprefix = False 'pogoj ki ga je potrebno dodati za več krogov izvedbe, bug pri prikazu na predavanjih
        ListBox1.Items.Clear()
        ListBox1.Items.Add("Vnesi besedilo za novo ime.")
        Me.Button2.Enabled = True
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click

        If isprefix = True Then
            'handle the case
            ListBox1.Items.Clear()
            ListBox1.Items.Add("Izbran je Prefix.")

            prefix = TextBox1.Text

            For Each foundFile As String In filelist
                filename = Path.GetFileName(foundFile)
                'dodaj extension
                extension = Path.GetExtension(foundFile)
                filename = prefix + filename + extension
                My.Computer.FileSystem.RenameFile(foundFile, filename)

            Next


            isprefix = False
            ListBox1.Items.Add("Preimenovanje s prefix je koncano")

        ElseIf isnewname = True Then
            'handle the case
            ListBox1.Items.Clear()
            ListBox1.Items.Add("Izbran je Newname.")

            newname = TextBox1.Text
            Dim newname2 As String = ""
            Dim index As String = 0
            Try
                For Each foundFile As String In filelist
                    If index = 0 Then
                        'File Exists Exception!
                        extension = Path.GetExtension(foundFile)
                        newname = newname + extension
                        My.Computer.FileSystem.RenameFile(foundFile, newname)
                    ElseIf index > 0 Then
                        'dodaj extension
                        extension = Path.GetExtension(foundFile)
                        newname2 = newname + index + extension
                        My.Computer.FileSystem.RenameFile(foundFile, newname2)
                    End If


                    index = index + 1
                Next
            Catch ex As Exception
                ' Show the exception's message.
                MessageBox.Show(ex.Message)


            End Try

            isnewname = False
            ListBox1.Items.Add("Preimenovanje z novim imenom je koncano")
        End If



    End Sub

    Private Sub btnFind_Click(sender As Object, e As EventArgs) Handles btnFind.Click
        iskana = ""
        ListBox1.Items.Clear()
        ListBox1.Items.Add("Vnesi ime iskane datoteke s končnico.")
        Me.Button5.Enabled = True

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim fi As New FileInfo(fpath) 'pridobi objekt za informacije o datoteki/mapi
        Dim folder As String
        folder = fi.Directory.ToString

        ListBox1.Items.Add(folder)
        recursivelist = IskanjeDatoteke(folder)

        For Each foundFile As String In recursivelist
            'ListBox1.Items.Add(foundFile)
            iskana = Me.TextBox1.Text
            najdena = False
            If foundFile.Contains(iskana) Then
                ListBox1.Items.Clear()
                ListBox1.Items.Add("Datoteka " & iskana & "je najdena v\n" & foundFile)
                najdena = True
            End If
            If najdena = False Then
                ListBox1.Items.Clear()
                ListBox1.Items.Add("Datoteka ni bila najdena.")
            End If

        Next
    End Sub
End Class
