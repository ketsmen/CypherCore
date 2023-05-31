// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: bgs/low/pb/client/club_name_generator.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Bgs.Protocol.Club.V1 {

  /// <summary>Holder for reflection information generated from bgs/low/pb/client/club_name_generator.proto</summary>
  public static partial class ClubNameGeneratorReflection {

    #region Descriptor
    /// <summary>File descriptor for bgs/low/pb/client/club_name_generator.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ClubNameGeneratorReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CitiZ3MvbG93L3BiL2NsaWVudC9jbHViX25hbWVfZ2VuZXJhdG9yLnByb3Rv",
            "EhRiZ3MucHJvdG9jb2wuY2x1Yi52MRohYmdzL2xvdy9wYi9jbGllbnQvY2x1",
            "Yl90eXBlLnByb3RvIuYBChNOYW1lR2VuZXJhdG9yQ29uZmlnEjwKD25hbWVf",
            "Z2VuZXJhdG9ycxgBIAMoCzIjLmJncy5wcm90b2NvbC5jbHViLnYxLk5hbWVH",
            "ZW5lcmF0b3ISSQoTY2x1Yl90eXBlX3Njb3JlY2FyZBgCIAEoCzIsLmJncy5w",
            "cm90b2NvbC5jbHViLnYxLk5hbWVHZW5lcmF0b3JTY29yZWNhcmQSRgoQbG9j",
            "YWxlX3Njb3JlY2FyZBgDIAEoCzIsLmJncy5wcm90b2NvbC5jbHViLnYxLk5h",
            "bWVHZW5lcmF0b3JTY29yZWNhcmQiiQEKFk5hbWVHZW5lcmF0b3JTY29yZWNh",
            "cmQSEwoLaXNfcmVxdWlyZWQYASABKAgSEgoKZnVsbF9tYXRjaBgCIAEoDRIV",
            "Cg1wYXJ0aWFsX21hdGNoGAMgASgNEhgKEHBhcnRpYWxfZmFsbGJhY2sYBCAB",
            "KA0SFQoNZnVsbF9mYWxsYmFjaxgFIAEoDSKvAQoNTmFtZUdlbmVyYXRvchIN",
            "CgVuYW1lcxgBIAMoCRJECgxyZXBsYWNlbWVudHMYAiADKAsyLi5iZ3MucHJv",
            "dG9jb2wuY2x1Yi52MS5OYW1lR2VuZXJhdG9yUmVwbGFjZW1lbnQSOAoKY2x1",
            "Yl90eXBlcxgDIAMoCzIkLmJncy5wcm90b2NvbC5jbHViLnYxLlVuaXF1ZUNs",
            "dWJUeXBlEg8KB2xvY2FsZXMYBCADKAkiNwoYTmFtZUdlbmVyYXRvclJlcGxh",
            "Y2VtZW50EgoKAmlkGAEgASgJEg8KB29wdGlvbnMYAiADKAlQAA=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Bgs.Protocol.Club.V1.ClubTypeReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Bgs.Protocol.Club.V1.NameGeneratorConfig), global::Bgs.Protocol.Club.V1.NameGeneratorConfig.Parser, new[]{ "NameGenerators", "ClubTypeScorecard", "LocaleScorecard" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Bgs.Protocol.Club.V1.NameGeneratorScorecard), global::Bgs.Protocol.Club.V1.NameGeneratorScorecard.Parser, new[]{ "IsRequired", "FullMatch", "PartialMatch", "PartialFallback", "FullFallback" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Bgs.Protocol.Club.V1.NameGenerator), global::Bgs.Protocol.Club.V1.NameGenerator.Parser, new[]{ "Names", "Replacements", "ClubTypes", "Locales" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Bgs.Protocol.Club.V1.NameGeneratorReplacement), global::Bgs.Protocol.Club.V1.NameGeneratorReplacement.Parser, new[]{ "Id", "Options" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class NameGeneratorConfig : pb::IMessage<NameGeneratorConfig> {
    private static readonly pb::MessageParser<NameGeneratorConfig> _parser = new pb::MessageParser<NameGeneratorConfig>(() => new NameGeneratorConfig());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NameGeneratorConfig> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Bgs.Protocol.Club.V1.ClubNameGeneratorReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorConfig() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorConfig(NameGeneratorConfig other) : this() {
      nameGenerators_ = other.nameGenerators_.Clone();
      clubTypeScorecard_ = other.HasClubTypeScorecard ? other.clubTypeScorecard_.Clone() : null;
      localeScorecard_ = other.HasLocaleScorecard ? other.localeScorecard_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorConfig Clone() {
      return new NameGeneratorConfig(this);
    }

    /// <summary>Field number for the "name_generators" field.</summary>
    public const int NameGeneratorsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Bgs.Protocol.Club.V1.NameGenerator> _repeated_nameGenerators_codec
        = pb::FieldCodec.ForMessage(10, global::Bgs.Protocol.Club.V1.NameGenerator.Parser);
    private readonly pbc::RepeatedField<global::Bgs.Protocol.Club.V1.NameGenerator> nameGenerators_ = new pbc::RepeatedField<global::Bgs.Protocol.Club.V1.NameGenerator>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Bgs.Protocol.Club.V1.NameGenerator> NameGenerators {
      get { return nameGenerators_; }
    }

    /// <summary>Field number for the "club_type_scorecard" field.</summary>
    public const int ClubTypeScorecardFieldNumber = 2;
    private global::Bgs.Protocol.Club.V1.NameGeneratorScorecard clubTypeScorecard_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Bgs.Protocol.Club.V1.NameGeneratorScorecard ClubTypeScorecard {
      get { return clubTypeScorecard_; }
      set {
        clubTypeScorecard_ = value;
      }
    }
    /// <summary>Gets whether the club_type_scorecard field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasClubTypeScorecard {
      get { return clubTypeScorecard_ != null; }
    }
    /// <summary>Clears the value of the club_type_scorecard field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearClubTypeScorecard() {
      clubTypeScorecard_ = null;
    }

    /// <summary>Field number for the "locale_scorecard" field.</summary>
    public const int LocaleScorecardFieldNumber = 3;
    private global::Bgs.Protocol.Club.V1.NameGeneratorScorecard localeScorecard_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Bgs.Protocol.Club.V1.NameGeneratorScorecard LocaleScorecard {
      get { return localeScorecard_; }
      set {
        localeScorecard_ = value;
      }
    }
    /// <summary>Gets whether the locale_scorecard field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasLocaleScorecard {
      get { return localeScorecard_ != null; }
    }
    /// <summary>Clears the value of the locale_scorecard field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearLocaleScorecard() {
      localeScorecard_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NameGeneratorConfig);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NameGeneratorConfig other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!nameGenerators_.Equals(other.nameGenerators_)) return false;
      if (!object.Equals(ClubTypeScorecard, other.ClubTypeScorecard)) return false;
      if (!object.Equals(LocaleScorecard, other.LocaleScorecard)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= nameGenerators_.GetHashCode();
      if (HasClubTypeScorecard) hash ^= ClubTypeScorecard.GetHashCode();
      if (HasLocaleScorecard) hash ^= LocaleScorecard.GetHashCode();
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
      nameGenerators_.WriteTo(output, _repeated_nameGenerators_codec);
      if (HasClubTypeScorecard) {
        output.WriteRawTag(18);
        output.WriteMessage(ClubTypeScorecard);
      }
      if (HasLocaleScorecard) {
        output.WriteRawTag(26);
        output.WriteMessage(LocaleScorecard);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += nameGenerators_.CalculateSize(_repeated_nameGenerators_codec);
      if (HasClubTypeScorecard) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ClubTypeScorecard);
      }
      if (HasLocaleScorecard) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(LocaleScorecard);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NameGeneratorConfig other) {
      if (other == null) {
        return;
      }
      nameGenerators_.Add(other.nameGenerators_);
      if (other.HasClubTypeScorecard) {
        if (!HasClubTypeScorecard) {
          ClubTypeScorecard = new global::Bgs.Protocol.Club.V1.NameGeneratorScorecard();
        }
        ClubTypeScorecard.MergeFrom(other.ClubTypeScorecard);
      }
      if (other.HasLocaleScorecard) {
        if (!HasLocaleScorecard) {
          LocaleScorecard = new global::Bgs.Protocol.Club.V1.NameGeneratorScorecard();
        }
        LocaleScorecard.MergeFrom(other.LocaleScorecard);
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
            nameGenerators_.AddEntriesFrom(input, _repeated_nameGenerators_codec);
            break;
          }
          case 18: {
            if (!HasClubTypeScorecard) {
              ClubTypeScorecard = new global::Bgs.Protocol.Club.V1.NameGeneratorScorecard();
            }
            input.ReadMessage(ClubTypeScorecard);
            break;
          }
          case 26: {
            if (!HasLocaleScorecard) {
              LocaleScorecard = new global::Bgs.Protocol.Club.V1.NameGeneratorScorecard();
            }
            input.ReadMessage(LocaleScorecard);
            break;
          }
        }
      }
    }

  }

  public sealed partial class NameGeneratorScorecard : pb::IMessage<NameGeneratorScorecard> {
    private static readonly pb::MessageParser<NameGeneratorScorecard> _parser = new pb::MessageParser<NameGeneratorScorecard>(() => new NameGeneratorScorecard());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NameGeneratorScorecard> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Bgs.Protocol.Club.V1.ClubNameGeneratorReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorScorecard() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorScorecard(NameGeneratorScorecard other) : this() {
      _hasBits0 = other._hasBits0;
      isRequired_ = other.isRequired_;
      fullMatch_ = other.fullMatch_;
      partialMatch_ = other.partialMatch_;
      partialFallback_ = other.partialFallback_;
      fullFallback_ = other.fullFallback_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorScorecard Clone() {
      return new NameGeneratorScorecard(this);
    }

    /// <summary>Field number for the "is_required" field.</summary>
    public const int IsRequiredFieldNumber = 1;
    private readonly static bool IsRequiredDefaultValue = false;

    private bool isRequired_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool IsRequired {
      get { if ((_hasBits0 & 1) != 0) { return isRequired_; } else { return IsRequiredDefaultValue; } }
      set {
        _hasBits0 |= 1;
        isRequired_ = value;
      }
    }
    /// <summary>Gets whether the "is_required" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasIsRequired {
      get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "is_required" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearIsRequired() {
      _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "full_match" field.</summary>
    public const int FullMatchFieldNumber = 2;
    private readonly static uint FullMatchDefaultValue = 0;

    private uint fullMatch_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint FullMatch {
      get { if ((_hasBits0 & 2) != 0) { return fullMatch_; } else { return FullMatchDefaultValue; } }
      set {
        _hasBits0 |= 2;
        fullMatch_ = value;
      }
    }
    /// <summary>Gets whether the "full_match" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasFullMatch {
      get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "full_match" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearFullMatch() {
      _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "partial_match" field.</summary>
    public const int PartialMatchFieldNumber = 3;
    private readonly static uint PartialMatchDefaultValue = 0;

    private uint partialMatch_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint PartialMatch {
      get { if ((_hasBits0 & 4) != 0) { return partialMatch_; } else { return PartialMatchDefaultValue; } }
      set {
        _hasBits0 |= 4;
        partialMatch_ = value;
      }
    }
    /// <summary>Gets whether the "partial_match" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasPartialMatch {
      get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "partial_match" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearPartialMatch() {
      _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "partial_fallback" field.</summary>
    public const int PartialFallbackFieldNumber = 4;
    private readonly static uint PartialFallbackDefaultValue = 0;

    private uint partialFallback_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint PartialFallback {
      get { if ((_hasBits0 & 8) != 0) { return partialFallback_; } else { return PartialFallbackDefaultValue; } }
      set {
        _hasBits0 |= 8;
        partialFallback_ = value;
      }
    }
    /// <summary>Gets whether the "partial_fallback" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasPartialFallback {
      get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "partial_fallback" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearPartialFallback() {
      _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "full_fallback" field.</summary>
    public const int FullFallbackFieldNumber = 5;
    private readonly static uint FullFallbackDefaultValue = 0;

    private uint fullFallback_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint FullFallback {
      get { if ((_hasBits0 & 16) != 0) { return fullFallback_; } else { return FullFallbackDefaultValue; } }
      set {
        _hasBits0 |= 16;
        fullFallback_ = value;
      }
    }
    /// <summary>Gets whether the "full_fallback" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasFullFallback {
      get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "full_fallback" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearFullFallback() {
      _hasBits0 &= ~16;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NameGeneratorScorecard);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NameGeneratorScorecard other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (IsRequired != other.IsRequired) return false;
      if (FullMatch != other.FullMatch) return false;
      if (PartialMatch != other.PartialMatch) return false;
      if (PartialFallback != other.PartialFallback) return false;
      if (FullFallback != other.FullFallback) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasIsRequired) hash ^= IsRequired.GetHashCode();
      if (HasFullMatch) hash ^= FullMatch.GetHashCode();
      if (HasPartialMatch) hash ^= PartialMatch.GetHashCode();
      if (HasPartialFallback) hash ^= PartialFallback.GetHashCode();
      if (HasFullFallback) hash ^= FullFallback.GetHashCode();
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
      if (HasIsRequired) {
        output.WriteRawTag(8);
        output.WriteBool(IsRequired);
      }
      if (HasFullMatch) {
        output.WriteRawTag(16);
        output.WriteUInt32(FullMatch);
      }
      if (HasPartialMatch) {
        output.WriteRawTag(24);
        output.WriteUInt32(PartialMatch);
      }
      if (HasPartialFallback) {
        output.WriteRawTag(32);
        output.WriteUInt32(PartialFallback);
      }
      if (HasFullFallback) {
        output.WriteRawTag(40);
        output.WriteUInt32(FullFallback);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasIsRequired) {
        size += 1 + 1;
      }
      if (HasFullMatch) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(FullMatch);
      }
      if (HasPartialMatch) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(PartialMatch);
      }
      if (HasPartialFallback) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(PartialFallback);
      }
      if (HasFullFallback) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(FullFallback);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NameGeneratorScorecard other) {
      if (other == null) {
        return;
      }
      if (other.HasIsRequired) {
        IsRequired = other.IsRequired;
      }
      if (other.HasFullMatch) {
        FullMatch = other.FullMatch;
      }
      if (other.HasPartialMatch) {
        PartialMatch = other.PartialMatch;
      }
      if (other.HasPartialFallback) {
        PartialFallback = other.PartialFallback;
      }
      if (other.HasFullFallback) {
        FullFallback = other.FullFallback;
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
          case 8: {
            IsRequired = input.ReadBool();
            break;
          }
          case 16: {
            FullMatch = input.ReadUInt32();
            break;
          }
          case 24: {
            PartialMatch = input.ReadUInt32();
            break;
          }
          case 32: {
            PartialFallback = input.ReadUInt32();
            break;
          }
          case 40: {
            FullFallback = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class NameGenerator : pb::IMessage<NameGenerator> {
    private static readonly pb::MessageParser<NameGenerator> _parser = new pb::MessageParser<NameGenerator>(() => new NameGenerator());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NameGenerator> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Bgs.Protocol.Club.V1.ClubNameGeneratorReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGenerator() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGenerator(NameGenerator other) : this() {
      names_ = other.names_.Clone();
      replacements_ = other.replacements_.Clone();
      clubTypes_ = other.clubTypes_.Clone();
      locales_ = other.locales_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGenerator Clone() {
      return new NameGenerator(this);
    }

    /// <summary>Field number for the "names" field.</summary>
    public const int NamesFieldNumber = 1;
    private static readonly pb::FieldCodec<string> _repeated_names_codec
        = pb::FieldCodec.ForString(10);
    private readonly pbc::RepeatedField<string> names_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> Names {
      get { return names_; }
    }

    /// <summary>Field number for the "replacements" field.</summary>
    public const int ReplacementsFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Bgs.Protocol.Club.V1.NameGeneratorReplacement> _repeated_replacements_codec
        = pb::FieldCodec.ForMessage(18, global::Bgs.Protocol.Club.V1.NameGeneratorReplacement.Parser);
    private readonly pbc::RepeatedField<global::Bgs.Protocol.Club.V1.NameGeneratorReplacement> replacements_ = new pbc::RepeatedField<global::Bgs.Protocol.Club.V1.NameGeneratorReplacement>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Bgs.Protocol.Club.V1.NameGeneratorReplacement> Replacements {
      get { return replacements_; }
    }

    /// <summary>Field number for the "club_types" field.</summary>
    public const int ClubTypesFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Bgs.Protocol.Club.V1.UniqueClubType> _repeated_clubTypes_codec
        = pb::FieldCodec.ForMessage(26, global::Bgs.Protocol.Club.V1.UniqueClubType.Parser);
    private readonly pbc::RepeatedField<global::Bgs.Protocol.Club.V1.UniqueClubType> clubTypes_ = new pbc::RepeatedField<global::Bgs.Protocol.Club.V1.UniqueClubType>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Bgs.Protocol.Club.V1.UniqueClubType> ClubTypes {
      get { return clubTypes_; }
    }

    /// <summary>Field number for the "locales" field.</summary>
    public const int LocalesFieldNumber = 4;
    private static readonly pb::FieldCodec<string> _repeated_locales_codec
        = pb::FieldCodec.ForString(34);
    private readonly pbc::RepeatedField<string> locales_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> Locales {
      get { return locales_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NameGenerator);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NameGenerator other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!names_.Equals(other.names_)) return false;
      if(!replacements_.Equals(other.replacements_)) return false;
      if(!clubTypes_.Equals(other.clubTypes_)) return false;
      if(!locales_.Equals(other.locales_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= names_.GetHashCode();
      hash ^= replacements_.GetHashCode();
      hash ^= clubTypes_.GetHashCode();
      hash ^= locales_.GetHashCode();
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
      names_.WriteTo(output, _repeated_names_codec);
      replacements_.WriteTo(output, _repeated_replacements_codec);
      clubTypes_.WriteTo(output, _repeated_clubTypes_codec);
      locales_.WriteTo(output, _repeated_locales_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += names_.CalculateSize(_repeated_names_codec);
      size += replacements_.CalculateSize(_repeated_replacements_codec);
      size += clubTypes_.CalculateSize(_repeated_clubTypes_codec);
      size += locales_.CalculateSize(_repeated_locales_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NameGenerator other) {
      if (other == null) {
        return;
      }
      names_.Add(other.names_);
      replacements_.Add(other.replacements_);
      clubTypes_.Add(other.clubTypes_);
      locales_.Add(other.locales_);
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
            names_.AddEntriesFrom(input, _repeated_names_codec);
            break;
          }
          case 18: {
            replacements_.AddEntriesFrom(input, _repeated_replacements_codec);
            break;
          }
          case 26: {
            clubTypes_.AddEntriesFrom(input, _repeated_clubTypes_codec);
            break;
          }
          case 34: {
            locales_.AddEntriesFrom(input, _repeated_locales_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class NameGeneratorReplacement : pb::IMessage<NameGeneratorReplacement> {
    private static readonly pb::MessageParser<NameGeneratorReplacement> _parser = new pb::MessageParser<NameGeneratorReplacement>(() => new NameGeneratorReplacement());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NameGeneratorReplacement> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Bgs.Protocol.Club.V1.ClubNameGeneratorReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorReplacement() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorReplacement(NameGeneratorReplacement other) : this() {
      id_ = other.id_;
      options_ = other.options_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NameGeneratorReplacement Clone() {
      return new NameGeneratorReplacement(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private readonly static string IdDefaultValue = "";

    private string id_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Id {
      get { return id_ ?? IdDefaultValue; }
      set {
        id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }
    /// <summary>Gets whether the "id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool HasId {
      get { return id_ != null; }
    }
    /// <summary>Clears the value of the "id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearId() {
      id_ = null;
    }

    /// <summary>Field number for the "options" field.</summary>
    public const int OptionsFieldNumber = 2;
    private static readonly pb::FieldCodec<string> _repeated_options_codec
        = pb::FieldCodec.ForString(18);
    private readonly pbc::RepeatedField<string> options_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> Options {
      get { return options_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NameGeneratorReplacement);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NameGeneratorReplacement other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if(!options_.Equals(other.options_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (HasId) hash ^= Id.GetHashCode();
      hash ^= options_.GetHashCode();
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
      if (HasId) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      options_.WriteTo(output, _repeated_options_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (HasId) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
      }
      size += options_.CalculateSize(_repeated_options_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NameGeneratorReplacement other) {
      if (other == null) {
        return;
      }
      if (other.HasId) {
        Id = other.Id;
      }
      options_.Add(other.options_);
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
            Id = input.ReadString();
            break;
          }
          case 18: {
            options_.AddEntriesFrom(input, _repeated_options_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
