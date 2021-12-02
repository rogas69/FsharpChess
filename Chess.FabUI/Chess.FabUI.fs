namespace Chess.FabUI

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms
open Chess.Domain
open Chess.Domain.Entities
open Xamarin.Forms.PlatformConfiguration
open System.Reflection
open System.IO


module App = 
    type Model = 
      { GameState: Entities.GameState 
        FromCell: Cell option }

    type Msg = 
        /// User will pick a "from" cell, and then a "to" cell.
        | PickCell of Cell

    let getImageBytes filename =
        match filename with
        | "" -> null
        | _ -> let assembly = IntrospectionExtensions.GetTypeInfo(typedefof<Model>).Assembly;
               use stream = assembly.GetManifestResourceStream(filename);
               use reader = new BinaryReader(stream)
               reader.ReadBytes(int stream.Length)
               

    let init () = 
        { GameState = Implementation.initGame(); FromCell = None }, Cmd.none

    let update msg model =
        match msg with
        | PickCell cell -> 
            match model.FromCell with
            | None -> // FromCell was not previously selected
                match model.GameState.Board.[cell] with
                | Some piece -> { model with FromCell = Some cell }, Cmd.none // Selected a cell has a piece, so update FromCell
                | None -> model, Cmd.none // Selected cell has no piece, so ignore it

            | Some fromCell -> // FromCell was already selected
                let gameState = Implementation.move model.GameState { AttemptedMove.FromCell = fromCell; AttemptedMove.ToCell = cell }
                { model with GameState = gameState; FromCell = None }, Cmd.none
        
    let view (model: Model) dispatch =

        let getCellBgColor cell colIdx rowIdx =
            if Some cell = model.FromCell then Color.LightGreen
            elif (colIdx + rowIdx) % 2 = 0 then Xamarin.Forms.Color.White
            else Color.LightBlue

        let getCellBorderColor cell =
            if Some cell = model.FromCell then Color.Green 
            else Color.Gray
        
        let indexedCells =
            let indexedCols = List.zip Entities.Column.List [0..7]
            let indexedRows = List.zip Entities.Row.List ([0..7] |> List.rev)

            [ for col, colIdx in indexedCols do
                for row, rowIdx in indexedRows do 
                    yield { Col = col; Row = row }, (colIdx, rowIdx) ]

        
        let imageForPiece pieceOpt = 
            match pieceOpt with
            | Some (color, rank) -> 
                let colorStr = match color with | White -> "white" | Black -> "black"
                let rankStr = match rank with | Pawn _ -> "pawn" | Rook -> "rook" | Bishop -> "bishop" | King -> "king" | Queen -> "queen" | Knight -> "knight"
                sprintf "Chess.FabUI.Resources.pieces_%s.%s.png" colorStr rankStr
            
            | None -> ""

        View.ContentPage(
            content = View.StackLayout( children = [ // <-- Fabulous.SimpleElements nuget package
                    View.Grid( 
                        rowdefs = [ for n in 1..8 -> Absolute 50. ],
                        coldefs = [ for n in 1..8 -> Absolute 50. ], 
                        columnSpacing = 0.,
                        rowSpacing = 0.,
                        children = [
                            for (cell, (colIdx, rowIdx)) in indexedCells do
                                let bgColor = getCellBgColor cell colIdx rowIdx
                                let borderColor = getCellBorderColor cell
                                let imageSource = imageForPiece model.GameState.Board.[cell]
                                let onTap = View.TapGestureRecognizer(command=(fun () -> dispatch (PickCell cell)))

                                yield View.Frame(
                                    backgroundColor = bgColor,
                                    borderColor = borderColor,
                                    gestureRecognizers = [onTap]
                                ).Row(rowIdx).Column(colIdx)

                                yield View.Image(
                                    source = Image.fromBytes (getImageBytes imageSource),
                                    gestureRecognizers = [onTap]
                                ).Row(rowIdx).Column(colIdx)
                            ]
                    )

                    View.Label( text = model.GameState.Message )
                ]
            )
        )

    // Note, this declaration is needed if you enable LiveUpdate
    let program = XamarinFormsProgram.mkProgram init update view




type App () as app = 
    inherit Application ()
    
    let runner = 
        App.program
        |> XamarinFormsProgram.run app
    
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


