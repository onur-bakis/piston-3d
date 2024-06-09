using System.Collections;
using System.Collections.Generic;
using Controller;
using Managers;
using Signals;
using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    
    public override void InstallBindings()
    {
        InstallModels();
        InstallSignals();
        InstallManager();
    }

    private void InstallManager()
    {
        Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        Container.BindInterfacesAndSelfTo<RaycastClickController>().AsSingle().NonLazy();
        Container.Bind<PistonSimStartController>().AsSingle().NonLazy();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<MouseLeftClickedSignal>();
        Container.DeclareSignal<MouseLeftDragSignal>();
        Container.DeclareSignal<MouseLeftButtonUpSignal>();
        Container.DeclareSignal<AssembleMovementAnimationFinished>();
        
        //Sim signal
        Container.DeclareSignal<CheckSimFinishSignal>();
        Container.DeclareSignal<SimFinishSignal>();
        
        //Ui signals
        Container.DeclareSignal<LearnNextPieceSignal>();
        Container.DeclareSignal<StartSimAnimationSignal>();
        Container.DeclareSignal<StartTheTestSignal>();
        Container.DeclareSignal<ResetSignal>();
    }

    private void InstallModels()
    {
        
    }
}