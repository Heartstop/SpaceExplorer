using Godot;
using System;

public class MessageDialog : Godot.PanelContainer
{
    private Action _callback = () => {};
    private RichTextLabel _message = null!;
    private Button _button = null!;
    private Vector2 _minSize = new Vector2(0,0);
    public override void _Ready()
    {
        Visible = false;
        _message = GetNode<RichTextLabel>("VBoxContainer/Message");
        _button = GetNode<Button>("VBoxContainer/ContinueButton");
        _minSize = RectMinSize;
        
        _button.Connect("gui_input", this, nameof(OnInput));
    }

    public void ShowMessage(string message, Action? callback = null)
    {
        void Noop(){};
        Visible = true;
        _callback = callback is null 
            ? Noop
            : callback;
        _button.GrabFocus();
        _message.BbcodeText = message;
    }

    public override void _Process(float delta)
    {
        if(Visible)
            SetSize(new Vector2(1,1));
    }
    
    public void OnInput(InputEvent inputEvent)
    {
        if(!inputEvent.IsActionReleased("ui_accept"))
            return;

        Visible = false;
        _callback();
    }
}
