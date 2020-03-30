/**
 * 描述:
 *    每帧执行一次的系统
 * @author ciao
 * @create 2020-01-08 17:17
 */

namespace Entitas
{
    public interface IUpdateSystem : Entitas.ISystem
    {
        void Update();
    }
}