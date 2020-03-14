﻿Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Text

Public Class GUIScriptManager
    Public HighlightBrush As SolidColorBrush = Brushes.SlateBlue


    Public Tabkeys As Dictionary(Of String, String)

    Public Sub New()


        Tabkeys = New Dictionary(Of String, String)
        Tabkeys.Add("Control", "#FFFFC500")
        Tabkeys.Add("plibFunc", "#FFCF2AFF")
        Tabkeys.Add("Condition", "#FF45EC5B")
        Tabkeys.Add("Action", "#FF2CC0EC")
        Tabkeys.Add("Func", "#FF6B66FF")
        Tabkeys.Add("Variable", "#FFFF6850")
        Tabkeys.Add("Value", "#FFFF8040")
        Tabkeys.Add("EtcBlock", "#FFCC3D3D")
    End Sub


    Public Function GetFuncList(tefile As TEFile) As List(Of String)
        Dim rstr As New List(Of String)

        Dim Script As GUIScriptEditor = tefile.Scripter



        For i = 0 To Script.items.Count - 1
            If Script.items(i).name = "function" Then
                If Script.items(i).value <> "onPluginStart" And Script.items(i).value <> "beforeTriggerExec" And Script.items(i).value <> "afterTriggerExec" Then
                    rstr.Add(Script.items(i).value)
                End If

            End If
        Next

        Return rstr
    End Function



    Public Function GetFuncInfor(name As String, Script As GUIScriptEditor) As ScriptBlock
        For i = 0 To Script.items.Count - 1
            If Script.items(i).name = "function" Then

                If Script.items(i).value = name Then
                    Return Script.items(i)
                End If
            End If
        Next
        Return Nothing
    End Function



    Public SCValueType() As String = {
    "TrgAllyStatus",
    "TrgComparison",
    "TrgCount",
    "TrgModifier",
    "TrgOrder",
    "TrgPlayer",
    "TrgProperty",
    "TrgPropState",
    "TrgResource",
    "TrgScore",
    "TrgSwitchAction",
    "TrgSwitchState",
    "TrgAIScript",
    "TrgLocation",
    "TrgLocationIndex",
    "TrgString",
    "TrgSwitch",
    "TrgUnit"}


    Public Function GetFuncArgs(scr As ScriptBlock) As List(Of ScriptBlock)
        If scr.name = "function" Then
            For i = 0 To scr.child.Count - 1
                If scr.child(i).name = "funargs" Then
                    Return scr.child(i).child
                End If
            Next
        End If


        Return Nothing
    End Function



    Public Function GetValueGroup(scr As ScriptBlock) As String



    End Function









    Public Shared Function ConvertTextToScript(text As String, Scripter As GUIScriptEditor) As List(Of ScriptBlock)
        Dim rScript As New List(Of ScriptBlock)


        Dim index As ULong = 1


        Dim order As String = ""

        While True
            If Mid(text, index).Trim = "" Then
                Exit While
            End If

            order = GetOrder(text, index)
            Select Case order
                Case "import"
                    Dim tstr As String = ""
                    tstr = GetOrder(text, index) & " " & GetOrder(text, index) & " " & GetOrder(text, index)
                    rScript.Add(New ScriptBlock("import", True, False, tstr, Scripter))
                Case "const"
                    Dim tstr As String = ""
                    tstr = ReadToLineEnd(text, index)
                    rScript.Add(New ScriptBlock("var", True, True, tstr, Scripter))
                Case "var"
                    Dim tstr As String = ""
                    tstr = ReadToLineEnd(text, index)
                    rScript.Add(New ScriptBlock("var", True, False, tstr, Scripter))
                Case "function"
                    Dim fname As String = GetOrder(text, index).Trim

                    Dim args As String = ReadToBracket(text, index).Trim
                    Dim body As String = ReadToBracket(text, index).Trim


                    Dim fsb As New ScriptBlock("function", True, False, fname, Scripter)
                    Dim fargs As New ScriptBlock("funargs", False, False, "", Scripter)
                    Dim argsarray() As String = args.Split(",")
                    For i = 0 To argsarray.Count - 1
                        fargs.AddChild(New ScriptBlock("", False, False, argsarray(i).Trim(), Scripter))
                    Next
                    fsb.AddChild(fargs)


                    rScript.Add(fsb)
                Case Else
                    '엑션등
                    Dim aname As String = order

                    Dim args As String = ReadToBracket(text, index).Trim
            End Select

        End While
        'rScript.Add(New ScriptBlock(tstr, True, False, "hf"))


        '1. 초기상태
        '글자가 나올 때 까지 반복
        '
        Return rScript
    End Function




    Public Shared Function ReadToBracket(text As String, ByRef index As ULong) As String
        Dim startindex As ULong = index
        Dim rtext As String = ""


        Dim bracketas As New List(Of String)

        Dim token As String = Mid(text, index, 1)

        While True
            rtext = rtext & token
            If token = "(" Or token = "{" Or token = "[" Then
                bracketas.Add(token)
            End If
            If token = ")" Or token = "}" Or token = "]" Then
                If (token = ")" And bracketas.Last = "(") Or (token = "]" And bracketas.Last = "[") Or (token = "}" And bracketas.Last = "{") Then
                    bracketas.RemoveAt(bracketas.Count - 1)
                End If
            End If
            index += 1
            token = Mid(text, index, 1)

            If bracketas.Count = 0 Then
                Exit While
            End If
            'MsgBox(rtext)
        End While




        Return Mid(rtext, 2, rtext.Length - 2)
    End Function

    Public Shared Function ReadToLineEnd(text As String, ByRef index As ULong) As String
        Dim token As String = Mid(text, index, 1)
        index += 1

        Dim order As String = ""
        While (token <> ";")
            order = order & token
            token = Mid(text, index, 1)

            index += 1
        End While


        Return order
    End Function

    Public Shared Function GetOrder(text As String, ByRef index As ULong) As String
        TrimText(text, index)

        Dim token As String = Mid(text, index, 1)
        Dim order As String = ""
        While token <> " " And token <> "(" And token <> "{" And token <> "	" And token <> vbCrLf And token <> ";"
            token = Mid(text, index, 1)
            order = order & token
            index += 1
        End While
        index -= 1

        If token = ";" Then
            index += 1
        End If

        Return order.Trim
    End Function

    Public Shared Sub TrimText(text As String, ByRef index As ULong)
        Dim token As String = Mid(text, index, 1)
        index += 1
        While (token.Trim = "" Or token = vbCrLf)
            token = Mid(text, index, 1)
            index += 1
        End While
        index -= 1
    End Sub




    'a .






    Public Shared Function GetIntend(intend As Integer) As String
        Dim str As String = ""

        For i = 0 To intend - 1
            str = str & "    "
        Next

        Return str
    End Function



    Public Shared Sub GetScriptText(scr As List(Of ScriptBlock), strb As StringBuilder, ByRef intend As Integer, Optional isCondition As Boolean = False, Optional isAnd As Boolean = True)
        For i = 0 To scr.Count - 1
            Dim sname As String = scr(i).name
            Dim svalue As String = scr(i).value
            Dim flag As Boolean = scr(i).flag
            Dim schild As List(Of ScriptBlock) = scr(i).child
            Dim isLastValue As Boolean = (i = scr.Count - 1)

            Select Case sname
                Case "import"
                    strb.Append(GetIntend(intend))
                    strb.Append("import ")
                    strb.Append(svalue)
                    strb.AppendLine(";")
                Case "rawcode"
                    'flag true = raw, false = intend

                    If flag Then
                        strb.AppendLine(svalue)
                    Else
                        Dim tv As String = svalue.Replace(vbLf, vbCrLf & GetIntend(intend))

                        strb.Append(GetIntend(intend))
                        strb.AppendLine(tv)
                    End If
                Case "var"
                    'flag true = isconst
                    strb.Append(GetIntend(intend))

                    If flag Then
                        strb.Append("const ")
                    Else
                        strb.Append("var ")
                    End If
                    strb.Append(svalue)
                    strb.AppendLine(";")
                Case "function"
                    strb.Append(GetIntend(intend))
                    strb.Append("function ")
                    strb.Append(svalue)


                    GetScriptText(schild, strb, intend)
                Case "funargs"
                    strb.Append("(")

                    GetScriptText(schild, strb, intend)

                    strb.Append(")")
                Case "funcontent"
                    strb.AppendLine("{")

                    intend += 1
                    GetScriptText(schild, strb, intend)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.AppendLine("}")
                Case "if"
                    strb.Append(GetIntend(intend))
                    strb.Append("if")

                    GetScriptText(schild, strb, intend)

                    strb.AppendLine("")
                Case "elseif"
                    strb.Append(GetIntend(intend))
                    strb.Append("else if")

                    GetScriptText(schild, strb, intend)

                    strb.AppendLine("")
                Case "ifcondition"
                    strb.AppendLine("(")

                    intend += 1
                    GetScriptText(schild, strb, intend, True)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.Append(")")
                Case "ifthen"
                    strb.AppendLine("{")

                    intend += 1
                    GetScriptText(schild, strb, intend)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.Append("}")
                Case "ifelse"
                    strb.AppendLine("else{")

                    intend += 1
                    GetScriptText(schild, strb, intend)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.Append("}")
                Case "for"
                    strb.Append(GetIntend(intend))
                    strb.Append("for")

                    GetScriptText(schild, strb, intend)
                Case "forcontent"
                    strb.Append("(")

                    GetScriptText(schild, strb, intend)

                    strb.Append(")")
                Case "forinit"
                    strb.Append(svalue)
                    strb.Append(" ; ")
                Case "forcondition"
                    strb.Append(svalue)
                    strb.Append(" ; ")
                Case "forincre"
                    strb.Append(svalue)
                Case "foraction"
                    strb.AppendLine("{")

                    intend += 1
                    GetScriptText(schild, strb, intend)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.AppendLine("}")
                Case "while"
                    strb.Append(GetIntend(intend))
                    strb.Append("while")

                    GetScriptText(schild, strb, intend)

                Case "whilecondition"
                    strb.AppendLine("(")

                    intend += 1
                    GetScriptText(schild, strb, intend, True)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.Append(")")
                Case "whileaction"
                    strb.AppendLine("{")

                    intend += 1
                    GetScriptText(schild, strb, intend)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.AppendLine("}")
                Case "switch"
                    strb.Append(GetIntend(intend))
                    strb.Append("switch")

                    intend += 1
                    GetScriptText(schild, strb, intend)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.AppendLine("}")
                Case "switchvar"
                    strb.Append("(")
                    strb.Append(svalue)
                    strb.Append(")")
                    strb.AppendLine("{")
                Case "switchcase"
                    'flag true = break
                    strb.Append(GetIntend(intend))
                    strb.Append("case ")
                    strb.Append(svalue)
                    strb.AppendLine(":")

                    intend += 1
                    GetScriptText(schild, strb, intend)
                    If flag Then
                        strb.Append(GetIntend(intend))
                        strb.AppendLine("break;")
                    End If
                    intend -= 1
                Case "or"
                    strb.Append(GetIntend(intend))
                    strb.AppendLine("(")

                    intend += 1
                    GetScriptText(schild, strb, intend, True, False)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.Append(")")

                    If Not isLastValue Then
                        If isAnd Then
                            strb.Append(" && ")
                        Else
                            strb.Append(" || ")
                        End If
                    End If
                    strb.AppendLine("")
                Case "and"
                    strb.Append(GetIntend(intend))
                    strb.AppendLine("(")

                    intend += 1
                    GetScriptText(schild, strb, intend, True, True)
                    intend -= 1

                    strb.Append(GetIntend(intend))
                    strb.Append(")")

                    If Not isLastValue Then
                        If isAnd Then
                            strb.Append(" && ")
                        Else
                            strb.Append(" || ")
                        End If
                    End If
                    strb.AppendLine("")
                Case "folder"
                    'flag true = iscondition


                    Dim temp As String() = svalue.Split("////")

                    Dim head As String = temp.First
                    Dim tail As String = temp.Last

                    strb.Append(GetIntend(intend))
                    strb.AppendLine(head)


                    intend += 1
                    GetScriptText(schild, strb, intend, flag)
                    intend -= 1


                    strb.Append(GetIntend(intend))
                    strb.AppendLine(tail)
                Case Else
                    '액션,조건등
                    If svalue = "" Then
                        strb.Append(GetIntend(intend))
                        strb.Append(sname)
                        strb.Append("(")

                        GetScriptText(schild, strb, intend)

                        strb.Append(")")
                        If isCondition Then
                            If Not isLastValue Then
                                If isAnd Then
                                    strb.Append(" && ")
                                Else
                                    strb.Append(" || ")
                                End If
                            End If
                            strb.AppendLine("")
                        Else
                            strb.AppendLine(";")
                        End If
                    Else
                        strb.Append(svalue)
                        If Not isLastValue Then
                            strb.Append(", ")
                        End If
                    End If
            End Select
        Next
    End Sub
End Class