using System;
using xnaMugen.IO;
using System.Collections.Generic;
using xnaMugen.Drawing;
using Microsoft.Xna.Framework;
using System.Linq;
using xnaMugen.Animations;
using xnaMugen.Audio;

namespace xnaMugen.Menus
{
    internal class Storyboard
    {
        public Storyboard(MenuSystem menuSystem, string path)
        {
            var textfile = menuSystem.GetSubSystem<FileSystem>().OpenTextFile(path);
            var animationManager = menuSystem.GetSubSystem<AnimationSystem>().CreateManager(textfile.Filepath);
            var sceneDef = textfile.GetSection("SceneDef");
            var directoryName = System.IO.Path.GetDirectoryName(path);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            var spritePath = $"{directoryName}/" + sceneDef.GetAttribute<string>("spr");
            var soundPath = $"{directoryName}/" + sceneDef.GetAttribute("snd", $"{fileName}.snd");
            var spritemanager = menuSystem.GetSubSystem<SpriteSystem>().CreateManager(spritePath);
            var soundManager = menuSystem.GetSubSystem<SoundSystem>().CreateManager(soundPath);
            var fontMap = new FontMap(new Dictionary<int, Font>());

            Vector2? position = null;
            Color? clearColor = null;

            m_scenes = new List<Scene>();
            var sceneSections = textfile.Where(o => o.Title.StartsWith("Scene ", StringComparison.OrdinalIgnoreCase));
            foreach (var sceneSection in sceneSections)
            {
                var scene = new Scene(menuSystem, sceneSection, spritemanager, animationManager, soundManager, fontMap);
                if (scene.Position.HasValue)
                {
                    position = scene.Position;
                }
                else if (position.HasValue)
                {
                    scene.Position = position;
                }
                if (scene.ClearColor.HasValue)
                {
                    clearColor = scene.ClearColor;
                }
                else if (clearColor.HasValue)
                {
                    scene.ClearColor = clearColor;
                }
                m_scenes.Add(scene);
            }
            m_index = sceneDef.GetAttribute("startscene", 0);
            if (m_index < 0)
            {
                m_index = 0;
            }
            if (m_index >= m_scenes.Count)
            {
                m_index = m_scenes.Count - 1;
            }
            m_scenes[m_index].Reset();
        }

        public void Update()
        {
            if (IsFinished) return;
            m_scenes[m_index].Update();
        }

        public void Draw()
        {
            if (IsFinished) return;
            m_scenes[m_index].Draw();
            if (m_scenes[m_index].IsFinished)
            {
                m_index++;
                if (m_index == m_scenes.Count)
                {
                    IsFinished = true;
                    return;
                }
                m_scenes[m_index].Reset();
            }
        }

        public bool IsFinished { get; private set; }

        private List<Scene> m_scenes;
        private int m_index;
    }
}