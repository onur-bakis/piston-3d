using System.Collections;
using System.Collections.Generic;
using Keys;
using Signals;
using UnityEngine;
using Zenject;

public class InputController  : ITickable,IInitializable
{
    [Inject] public SignalBus _signalBus;

    private InputDataParams _inputDataParams;
    private float _clickInterval = 0.4f;
    private float _lastClickTime;

    private MouseLeftClickedSignal _mouseLeftClickedSignal;
    private MouseLeftDragSignal _mouseLeftDragSignal;
    private MouseLeftButtonUpSignal _mouseLeftButtonUpSignal;

    private bool inGame;
    
    public void Initialize()
    {
        _lastClickTime = int.MinValue;
        _mouseLeftClickedSignal = new MouseLeftClickedSignal();
        _mouseLeftDragSignal = new MouseLeftDragSignal();
        _mouseLeftButtonUpSignal = new MouseLeftButtonUpSignal();
        
        _signalBus.Subscribe<StartTheTestSignal>(TestStarted);
        _signalBus.Subscribe<ResetSignal>(TestEnded);
        _signalBus.Subscribe<SimFinishSignal>(TestEnded);
    }

    private void TestStarted()
    {
        inGame = true;
    }

    private void TestEnded()
    {
        inGame = false;
    }

    public void Tick()
    {
        if(!inGame)
            return;
        
        if(Input.GetMouseButtonDown(0))
        {
            OnMouseLeftClickDown();
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseLeftClickUp();
        }
    }

    private void OnMouseLeftClickDown()
    {
        _inputDataParams.inputPoint = Input.mousePosition;
        _mouseLeftClickedSignal.inputDataParams = _inputDataParams;
        
        _signalBus.Fire(_mouseLeftClickedSignal);
    }

    private void OnMouseDrag()
    {
        _inputDataParams.inputPoint = Input.mousePosition;
        _mouseLeftDragSignal.inputDataParams = _inputDataParams;
        
        _signalBus.Fire(_mouseLeftDragSignal);
    }

    private void OnMouseLeftClickUp()
    {
        _signalBus.Fire(_mouseLeftButtonUpSignal);
    }
    
}
