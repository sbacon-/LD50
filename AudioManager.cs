using System;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] sounds;
    float volume;
    bool mute,musicMute;

    void Awake(){
        if(Instance==null){
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this){
            Destroy(gameObject);
            return;
        }
        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.name = s.name;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.mute = s.mute;
        }
    }

    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s==null){
            Debug.Log("Unable to find Sound: "+name);
            return;
        }
        if(!s.source.isPlaying)s.source.Play();
        Debug.Log("Playing - "+name);
    }

    public void UpdateAudio(){

        volume = GameObject.Find("Volume").GetComponent<Slider>().value;
        mute = GameObject.Find("Mute").GetComponent<Toggle>().isOn;
        musicMute = GameObject.Find("Music Mute").GetComponent<Toggle>().isOn; 

        foreach(Sound s in sounds){
            s.source.volume = s.loop?2.0f*volume/3.0f:volume;
            s.source.mute = mute;
            if(s.loop)s.source.mute = musicMute;
        }
    }

    public void Stop(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s==null){
            Debug.Log("Unable to find Sound: "+name);
            return;
        }
        if(s.source.isPlaying)s.source.Stop();
    }


}
