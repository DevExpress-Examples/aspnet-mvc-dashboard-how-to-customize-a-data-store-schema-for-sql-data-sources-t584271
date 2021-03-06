﻿Imports System.Collections.Specialized
Imports System.Web.Routing
Imports DevExpress.DashboardWeb
Imports DevExpress.DashboardWeb.Mvc
Imports DevExpress.DataAccess.Sql
Imports DevExpress.Xpo.DB

Namespace MvcDashboard_CustomSchemaProvider
    Public NotInheritable Class DashboardConfig

        Private Sub New()
        End Sub

        Public Shared Sub RegisterService(ByVal routes As RouteCollection)
            routes.MapDashboardRoute("dashboardControl")

            Dim dashboardFileStorage As New DashboardFileStorage("~/App_Data/Dashboards")
            DashboardConfigurator.Default.SetDashboardStorage(dashboardFileStorage)

            DashboardConfigurator.Default.SetDBSchemaProvider(New CustomDBSchemaProvider())
        End Sub
    End Class

    ' Creates a new class that defines a custom data store schema by implementing the 
    ' IDBSchemaProvider interface.
    Friend Class CustomDBSchemaProvider
        Implements IDBSchemaProviderEx

        Private tables() As DBTable
        Public Sub LoadColumns(ByVal connection As SqlDataConnection, ParamArray ByVal tables() As DBTable) _
            Implements IDBSchemaProviderEx.LoadColumns
            For Each table As DBTable In tables
                If table.Name = "Categories" AndAlso table.Columns.Count = 0 Then
                    Dim categoryIdColumn As DBColumn = New DBColumn With {.Name = "CategoryID"}
                    table.AddColumn(categoryIdColumn)
                    Dim categoryNameColumn As DBColumn = New DBColumn With {.Name = "CategoryName"}
                    table.AddColumn(categoryNameColumn)
                End If
                If table.Name = "Products" AndAlso table.Columns.Count = 0 Then
                    Dim categoryIdColumn As DBColumn = New DBColumn With {.Name = "CategoryID"}
                    table.AddColumn(categoryIdColumn)
                    Dim productNameColumn As DBColumn = New DBColumn With {.Name = "ProductName"}
                    table.AddColumn(productNameColumn)

                    Dim foreignKey As New DBForeignKey({categoryIdColumn}, "Categories", CustomDBSchemaProvider.CreatePrimaryKeys("CategoryID"))
                    table.ForeignKeys.Add(foreignKey)
                End If
            Next table
        End Sub

        Public Shared Function CreatePrimaryKeys(ParamArray ByVal names() As String) As StringCollection
            Dim collection As New StringCollection()
            collection.AddRange(names)
            Return collection
        End Function

        Public Function GetTables(ByVal connection As SqlDataConnection, ParamArray ByVal tableList() As String) As DBTable() _
            Implements IDBSchemaProviderEx.GetTables
            If connection.Name = "nwindConnection" Then
                If tables IsNot Nothing Then
                    Return tables
                End If
                tables = New DBTable(1) {}

                Dim categoriesTable As New DBTable("Categories")
                tables(0) = categoriesTable

                Dim productsTable As New DBTable("Products")
                tables(1) = productsTable
            Else
                tables = New DBTable() {}
            End If
            Return tables
        End Function

        Public Function GetViews(ByVal connection As SqlDataConnection, ParamArray ByVal viewList() As String) As DBTable() _
            Implements IDBSchemaProviderEx.GetViews
            Dim views(-1) As DBTable
            Return views
        End Function

        Public Function GetProcedures(ByVal connection As SqlDataConnection, ParamArray ByVal procedureList() As String) As DBStoredProcedure() _
            Implements IDBSchemaProviderEx.GetProcedures
            Dim storedProcedures(-1) As DBStoredProcedure
            Return storedProcedures
        End Function
    End Class
End Namespace