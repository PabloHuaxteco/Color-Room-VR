using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ColorRoomVR
{
    /// <summary>
    /// Animates the visual change when the player paints. The paint itself (saving, events) is already done
    /// by the time Play is called; this only replays the rendered color from the old value to the new one.
    /// </summary>
    public class PaintEffectManager : MonoBehaviour
    {
        public enum Mode { None, Tween }

        // Private serialized fields
        [SerializeField] private Mode mode = Mode.Tween;
        [SerializeField, Min(0.01f)] private float duration = 0.5f;
        [SerializeField] private Ease ease = Ease.OutQuad;

        // Private fields
        private readonly List<PaintableObject> _targets = new();
        private readonly List<Color> _fromColors = new();

        /// <summary>
        /// Call right before painting: remembers the colors currently rendered by the objects that the paint
        /// will affect (a group = all its members), since SetColor applies the new color immediately.
        /// </summary>
        public void Capture(PaintableObject obj, PaintableGroup group)
        {
            _targets.Clear();
            _fromColors.Clear();

            if (group != null)
                _targets.AddRange(group.Members);
            else if (obj != null)
                _targets.Add(obj);

            foreach (var t in _targets)
                _fromColors.Add(t != null ? t.VisualColor : Color.white);
        }

        /// <summary>Call right after painting, to animate from the captured colors to the new one.</summary>
        public void Play(Color to)
        {
            if (mode == Mode.None)
                return;

            for (int i = 0; i < _targets.Count; i++)
            {
                var target = _targets[i];
                if (target == null)
                    continue;

                // Targeting the component (not the transform) so other tweens on the object are left alone.
                target.DOKill();
                PlayTween(target, _fromColors[i], to);
            }
        }

        private void PlayTween(PaintableObject target, Color from, Color to)
        {
            // The final color was already applied by SetColor; rewind to the start and blend forward.
            target.ApplyVisualColor(from);

            DOVirtual.Float(0f, 1f, duration, t => target.ApplyVisualColor(Color.Lerp(from, to, t)))
                .SetEase(ease)
                .SetTarget(target)
                .SetLink(target.gameObject)
                .OnKill(() =>
                {
                    // Never leave a half-blended color if the tween is cut short by a new paint or by disabling.
                    if (target != null)
                        target.ApplyVisualColor(to);
                });
        }
    }
}
