namespace FastWiki.Service.Contracts.Users.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// �˻�
    /// </summary>
    public string Account { get;  set; }

    /// <summary>
    /// �ǳ�
    /// </summary>
    public string Name { get;  set; }

    /// <summary>
    /// ����
    /// </summary>
    public string Password { get;  set; }

    /// <summary>
    /// ������
    /// </summary>
    public string Salt { get;  set; }

    /// <summary>
    /// ͷ��
    /// </summary>
    public string Avatar { get;  set; }

    /// <summary>
    /// ����
    /// </summary>
    public string Email { get;  set; }

    /// <summary>
    /// �ֻ���
    /// </summary>
    public string Phone { get;  set; }

    /// <summary>
    /// �Ƿ����
    /// </summary>
    public bool IsDisable { get;  set; }
}