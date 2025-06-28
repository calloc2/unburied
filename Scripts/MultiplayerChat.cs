using Godot;

public partial class MultiplayerChat : Control
{
    [Export] private NodePath lobbyPath; // Caminho para o nó Lobby
    private Lobby lobby; // Referência ao script Lobby
    private string playerName; // Nome do jogador local
    private TextEdit chatBox => GetNode<TextEdit>("TextEdit");
    private LineEdit msgBox => GetNode<LineEdit>("VBoxContainer/Message");

    public override void _Ready()
    {
        // Obtém a referência ao nó Lobby
        lobby = GetNode<Lobby>(lobbyPath);
        // Obtém o nome inicial do jogador
        playerName = lobby.GetLocalPlayerName();
        // Conecta o sinal TextChanged do NameEdit do Lobby
        lobby.NameEdit.TextChanged += OnLobbyNameChanged;
        // Conecta o evento de envio de mensagem
        msgBox.TextSubmitted += OnSendMessage;
    }

    private void OnLobbyNameChanged(string newText)
    {
        // Atualiza o playerName sempre que o nome no Lobby mudar
        playerName = lobby.GetLocalPlayerName();
    }

    private void OnSendMessage(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            Rpc(nameof(MessageRPC), playerName, text);
            msgBox.Text = ""; // Limpa o campo de mensagem após o envio
        }

        chatBox.ScrollVertical = chatBox.GetLineCount();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void MessageRPC(string usr, string data)
    {
        chatBox.Text += $"{usr}: {data}\n";
    }
}