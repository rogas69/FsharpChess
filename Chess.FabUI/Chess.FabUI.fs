﻿// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace Chess.FabUI

open System.Diagnostics
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms
open Chess.Domain
open Chess.Domain.Entities

module App = 
    type Model = 
      { GameState: Entities.GameState }

    type Msg = 
        | Move of Entities.AttemptedMove

    let initModel = { GameState = Implementation.initGame() }

    let init () = initModel, Cmd.none

    let update msg model =
        match msg with
        | Move move -> { model with GameState = Implementation.move model.GameState move }, Cmd.none
        
    let indexedCells =
        let indexedCols = List.zip Entities.Column.List [0..7]
        let indexedRows = List.zip Entities.Row.List [0..7]
        [ for col, colIdx in indexedCols do
            for row, rowIdx in indexedRows do 
                yield { Col = col; Row = row }, (colIdx, rowIdx) ]

    let imageForPiece pieceOpt = 
        match pieceOpt with
        | Some (color, rank) -> 
            let colorStr = match color with | White -> "white" | Black -> "black"
            let rankStr = match rank with | Pawn _ -> "pawn" | Rook -> "rook" | Bishop -> "bishop" | King -> "king" | Queen -> "queen" | Knight -> "knight"
            sprintf "/Images/pieces_%s/%s.png" colorStr rankStr
        | None -> 
            ""

    let view (model: Model) dispatch =
        View.ContentPage(widthRequest=416.,heightRequest=460.,
            content = View.StackLayout(
                children = [
                    View.Grid(
                        rowdefs=[50.; 50.; 50.; 50.; 50.; 50.; 50.; 50.],
                        coldefs=[50.; 50.; 50.; 50.; 50.; 50.; 50.; 50.],
                        children=[                            
                            for (cell, (colIdx, rowIdx)) in indexedCells do
                                let imageSource = imageForPiece model.GameState.Board.[cell]
                                yield 
                                    View.Button(backgroundColor=Color.LightBlue).Content(
                                        View.Image(source=imageSource, horizontalOptions=LayoutOptions.Center, verticalOptions=LayoutOptions.Center, margin=5.)
                                    ).GridColumn(colIdx).GridRow(rowIdx)
                        ]
                    )

                    View.Label(model.GameState.Message)
                ]
            )
        )

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/tools.html for further  instructions.
    //
    do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/models.html for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif


