using System;
using Godot;
using Godot.Collections;

public partial class AudioManager : Node
{
    private AudioStreamPlayer _input => GetNode<AudioStreamPlayer>("Input");
    private int index;
    private AudioEffectCapture effect;
    private AudioStreamGeneratorPlayback playback => GetNode<AudioStreamPlayer3D>(AudioOutputPath).GetStreamPlayback() as AudioStreamGeneratorPlayback;
    [Export] public NodePath AudioOutputPath { get; set; }
    [Export] public float inputThreashold = 0.005f;
    private Array<float> receiveBuffer = new Array<float>();

    public override void _Process(double delta)
    {
        if (IsMultiplayerAuthority())
            processMic();
        
        processVoice();
    }

    public void SetupAudio(long id)
    {
        if (IsMultiplayerAuthority())
        {
            _input.Stream = new AudioStreamMicrophone();
            _input.Play();

            index = AudioServer.GetBusIndex("Record");
            effect = AudioServer.GetBusEffect(index, 0) as AudioEffectCapture;
        }
    }

    private void processVoice()
    {
        if (receiveBuffer.Count <= 0) return;

        for (int i = 0; i < Mathf.Min(playback.GetFramesAvailable(), receiveBuffer.Count); i++)
        {
            playback.PushFrame(new Vector2(receiveBuffer[0], receiveBuffer[0]));
            receiveBuffer.RemoveAt(0);
        }
    }
    private void processMic()
    {
        var stereoData = effect.GetBuffer(effect.GetFramesAvailable());

        float maxAmplitude = 0.0f;

        if (stereoData.Length > 0)
        {
            var data = new float[stereoData.Length];
            for (int i = 0; i < stereoData.Length; i++)
            {
                var value = (stereoData[i].X + stereoData[i].Y) / 2;
                data[i] = value;

                maxAmplitude = Mathf.Max(value, maxAmplitude);
            }

            if (maxAmplitude < inputThreashold) return;

            Rpc("sendData", data);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    private void sendData(float[] data)
    {
        receiveBuffer.AddRange(data);
    }
}
