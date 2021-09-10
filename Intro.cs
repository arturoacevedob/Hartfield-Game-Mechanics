using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{

    VideoPlayer reproductor;

    void Awake(){
        reproductor = gameObject.GetComponent<VideoPlayer>();
        reproductor.loopPointReached += TerminoElVideo;
        reproductor.Play();
    }

   void Update()
    {
        if (Input.GetButton("Back"))
        {
            SceneManager.LoadScene("Main");
        } 
    }

    void TerminoElVideo(VideoPlayer vp)
    {
        
        
            vp.Stop();
        SceneManager.LoadScene("Main");
        

    }
}
