/**
 * 描述:
 *    固定时间间隔执行一次的系统
 * @author ciao
 * @create 2020-01-08 17:17
 */

namespace Entitas
{
    public interface IFixedUpdateSystem : Entitas.ISystem
    {
        void FixedUpdate();
    }
}