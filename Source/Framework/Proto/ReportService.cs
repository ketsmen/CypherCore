// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: bgs/low/pb/client/report_service.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Bgs.Protocol.Report.V1 {

  /// <summary>Holder for reflection information generated from bgs/low/pb/client/report_service.proto</summary>
  public static partial class ReportServiceReflection {

    #region Descriptor
    /// <summary>File descriptor for bgs/low/pb/client/report_service.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ReportServiceReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiZiZ3MvbG93L3BiL2NsaWVudC9yZXBvcnRfc2VydmljZS5wcm90bxIWYmdz",
            "LnByb3RvY29sLnJlcG9ydC52MRolYmdzL2xvdy9wYi9jbGllbnQvYWNjb3Vu",
            "dF90eXBlcy5wcm90bxokYmdzL2xvdy9wYi9jbGllbnQvcmVwb3J0X3R5cGVz",
            "LnByb3RvGiFiZ3MvbG93L3BiL2NsaWVudC9ycGNfdHlwZXMucHJvdG8aOWJn",
            "cy9sb3cvcGIvY2xpZW50L2dsb2JhbF9leHRlbnNpb25zL21lc3NhZ2Vfb3B0",
            "aW9ucy5wcm90bxo3YmdzL2xvdy9wYi9jbGllbnQvZ2xvYmFsX2V4dGVuc2lv",
            "bnMvZmllbGRfb3B0aW9ucy5wcm90bxo4YmdzL2xvdy9wYi9jbGllbnQvZ2xv",
            "YmFsX2V4dGVuc2lvbnMvbWV0aG9kX29wdGlvbnMucHJvdG8aOWJncy9sb3cv",
            "cGIvY2xpZW50L2dsb2JhbF9leHRlbnNpb25zL3NlcnZpY2Vfb3B0aW9ucy5w",
            "cm90byJcChFTZW5kUmVwb3J0UmVxdWVzdBIuCgZyZXBvcnQYASACKAsyHi5i",
            "Z3MucHJvdG9jb2wucmVwb3J0LnYxLlJlcG9ydBIPCgdwcm9ncmFtGAIgASgN",
            "OgaC+SsCEAEipQEKE1N1Ym1pdFJlcG9ydFJlcXVlc3QSPAoIYWdlbnRfaWQY",
            "ASABKAsyKi5iZ3MucHJvdG9jb2wuYWNjb3VudC52MS5HYW1lQWNjb3VudEhh",
            "bmRsZRI3CgtyZXBvcnRfdHlwZRgCIAEoCzIiLmJncy5wcm90b2NvbC5yZXBv",
            "cnQudjEuUmVwb3J0VHlwZRIPCgdwcm9ncmFtGAMgASgNOgaC+SsCEAEy/QEK",
            "DVJlcG9ydFNlcnZpY2USVQoKU2VuZFJlcG9ydBIpLmJncy5wcm90b2NvbC5y",
            "ZXBvcnQudjEuU2VuZFJlcG9ydFJlcXVlc3QaFC5iZ3MucHJvdG9jb2wuTm9E",
            "YXRhIgaC+SsCCAESWQoMU3VibWl0UmVwb3J0EisuYmdzLnByb3RvY29sLnJl",
            "cG9ydC52MS5TdWJtaXRSZXBvcnRSZXF1ZXN0GhQuYmdzLnByb3RvY29sLk5v",
            "RGF0YSIGgvkrAggCGjqC+SskCiJibmV0LnByb3RvY29sLnJlcG9ydC5SZXBv",
            "cnRTZXJ2aWNlgvkrCCoGcmVwb3J0ivkrAhABQgOAAQA="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Bgs.Protocol.Account.V1.AccountTypesReflection.Descriptor, global::Bgs.Protocol.Report.V1.ReportTypesReflection.Descriptor, global::Bgs.Protocol.RpcTypesReflection.Descriptor, global::Bgs.Protocol.MessageOptionsReflection.Descriptor, global::Bgs.Protocol.FieldOptionsReflection.Descriptor, global::Bgs.Protocol.MethodOptionsReflection.Descriptor, global::Bgs.Protocol.ServiceOptionsReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Bgs.Protocol.Report.V1.SendReportRequest), global::Bgs.Protocol.Report.V1.SendReportRequest.Parser, new[]{ "Report", "Program" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Bgs.Protocol.Report.V1.SubmitReportRequest), global::Bgs.Protocol.Report.V1.SubmitReportRequest.Parser, new[]{ "AgentId", "ReportType", "Program" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class SendReportRequest : pb::IMessage<SendReportRequest> {
    private static readonly pb::MessageParser<SendReportRequest> _parser = new pb::MessageParser<SendReportRequest>(() => new SendReportRequest());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SendReportRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Bgs.Protocol.Report.V1.ReportServiceReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SendReportRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SendReportRequest(SendReportRequest other) : this() {
      _hasBits0 = other._hasBits0;
      report_ = other.HasReport ? other.report_.Clone() : null;
      program_ = other.program_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SendReportRequest Clone() {
      return new SendReportRequest(this);
    }

    /// <summary>Field number for the "report" field.</summary>
    public const int ReportFieldNumber = 1;
    private global::Bgs.Protocol.Report.V1.Report report_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Bgs.Protocol.Report.V1.Report Report {
      get { return report_; }
      set {
        report_ = value;
      }
    }
    /// <summary>Gets whether the report field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasReport {
      get { return report_ != null; }
    }
    /// <summary>Clears the value of the report field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearReport() {
      report_ = null;
    }

    /// <summary>Field number for the "program" field.</summary>
    public const int ProgramFieldNumber = 2;
    private readonly static uint ProgramDefaultValue = 0;

    private uint program_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Program {
      get { if ((_hasBits0 & 1) != 0) { return program_; } else { return ProgramDefaultValue; } }
      set {
        _hasBits0 |= 1;
        program_ = value;
      }
    }
    /// <summary>Gets whether the "program" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasProgram {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "program" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearProgram() {
      _hasBits0 &= ~1;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SendReportRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SendReportRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Report, other.Report)) return false;
      if (Program != other.Program) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasReport) hash ^= Report.GetHashCode();
      if (HasProgram) hash ^= Program.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (HasReport) {
        output.WriteRawTag(10);
        output.WriteMessage(Report);
      }
      if (HasProgram) {
        output.WriteRawTag(16);
        output.WriteUInt32(Program);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasReport) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Report);
      }
      if (HasProgram) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Program);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SendReportRequest other) {
      if (other == null) {
        return;
      }
      if (other.HasReport) {
        if (!HasReport) {
          Report = new global::Bgs.Protocol.Report.V1.Report();
        }
        Report.MergeFrom(other.Report);
      }
      if (other.HasProgram) {
        Program = other.Program;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (!HasReport) {
              Report = new global::Bgs.Protocol.Report.V1.Report();
            }
            input.ReadMessage(Report);
            break;
          }
          case 16: {
            Program = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class SubmitReportRequest : pb::IMessage<SubmitReportRequest> {
    private static readonly pb::MessageParser<SubmitReportRequest> _parser = new pb::MessageParser<SubmitReportRequest>(() => new SubmitReportRequest());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<SubmitReportRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Bgs.Protocol.Report.V1.ReportServiceReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SubmitReportRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SubmitReportRequest(SubmitReportRequest other) : this() {
      _hasBits0 = other._hasBits0;
      agentId_ = other.HasAgentId ? other.agentId_.Clone() : null;
      reportType_ = other.HasReportType ? other.reportType_.Clone() : null;
      program_ = other.program_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public SubmitReportRequest Clone() {
      return new SubmitReportRequest(this);
    }

    /// <summary>Field number for the "agent_id" field.</summary>
    public const int AgentIdFieldNumber = 1;
    private global::Bgs.Protocol.Account.V1.GameAccountHandle agentId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Bgs.Protocol.Account.V1.GameAccountHandle AgentId {
      get { return agentId_; }
      set {
        agentId_ = value;
      }
    }
    /// <summary>Gets whether the agent_id field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasAgentId {
      get { return agentId_ != null; }
    }
    /// <summary>Clears the value of the agent_id field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearAgentId() {
      agentId_ = null;
    }

    /// <summary>Field number for the "report_type" field.</summary>
    public const int ReportTypeFieldNumber = 2;
    private global::Bgs.Protocol.Report.V1.ReportType reportType_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Bgs.Protocol.Report.V1.ReportType ReportType {
      get { return reportType_; }
      set {
        reportType_ = value;
      }
    }
    /// <summary>Gets whether the report_type field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasReportType {
      get { return reportType_ != null; }
    }
    /// <summary>Clears the value of the report_type field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearReportType() {
      reportType_ = null;
    }

    /// <summary>Field number for the "program" field.</summary>
    public const int ProgramFieldNumber = 3;
    private readonly static uint ProgramDefaultValue = 0;

    private uint program_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Program {
      get { if ((_hasBits0 & 1) != 0) { return program_; } else { return ProgramDefaultValue; } }
      set {
        _hasBits0 |= 1;
        program_ = value;
      }
    }
    /// <summary>Gets whether the "program" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasProgram {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "program" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearProgram() {
      _hasBits0 &= ~1;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as SubmitReportRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(SubmitReportRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(AgentId, other.AgentId)) return false;
      if (!object.Equals(ReportType, other.ReportType)) return false;
      if (Program != other.Program) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasAgentId) hash ^= AgentId.GetHashCode();
      if (HasReportType) hash ^= ReportType.GetHashCode();
      if (HasProgram) hash ^= Program.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (HasAgentId) {
        output.WriteRawTag(10);
        output.WriteMessage(AgentId);
      }
      if (HasReportType) {
        output.WriteRawTag(18);
        output.WriteMessage(ReportType);
      }
      if (HasProgram) {
        output.WriteRawTag(24);
        output.WriteUInt32(Program);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasAgentId) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(AgentId);
      }
      if (HasReportType) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ReportType);
      }
      if (HasProgram) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Program);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(SubmitReportRequest other) {
      if (other == null) {
        return;
      }
      if (other.HasAgentId) {
        if (!HasAgentId) {
          AgentId = new global::Bgs.Protocol.Account.V1.GameAccountHandle();
        }
        AgentId.MergeFrom(other.AgentId);
      }
      if (other.HasReportType) {
        if (!HasReportType) {
          ReportType = new global::Bgs.Protocol.Report.V1.ReportType();
        }
        ReportType.MergeFrom(other.ReportType);
      }
      if (other.HasProgram) {
        Program = other.Program;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (!HasAgentId) {
              AgentId = new global::Bgs.Protocol.Account.V1.GameAccountHandle();
            }
            input.ReadMessage(AgentId);
            break;
          }
          case 18: {
            if (!HasReportType) {
              ReportType = new global::Bgs.Protocol.Report.V1.ReportType();
            }
            input.ReadMessage(ReportType);
            break;
          }
          case 24: {
            Program = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
