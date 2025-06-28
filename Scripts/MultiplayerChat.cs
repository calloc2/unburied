using Godot;

public partial class MultiplayerChat : Control
{
    [Export] private NodePath lobbyPath;
    private Lobby lobby;
    private string playerName;
    private TextEdit chatBox => GetNode<TextEdit>("TextEdit");
    private LineEdit msgBox => GetNode<LineEdit>("VBoxContainer/Message");

    public override void _Ready()
    {
        lobby = GetNode<Lobby>(lobbyPath);
        playerName = lobby.GetLocalPlayerName();
        lobby.NameEdit.TextChanged += OnLobbyNameChanged;
        msgBox.TextSubmitted += OnSendMessage;
    }

    private void OnLobbyNameChanged(string newText)
    {
        playerName = lobby.GetLocalPlayerName();
    }

    private void OnSendMessage(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            Rpc(nameof(MessageRPC), playerName, text);
            msgBox.Text = "";
        }

        chatBox.ScrollVertical = chatBox.GetLineCount();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void MessageRPC(string usr, string data)
    {
        chatBox.Text += $"{usr}: {data}\n";
    }
}