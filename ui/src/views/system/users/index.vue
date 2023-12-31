<script setup lang="ts">
import { PureTable } from "@pureadmin/table";
import { useRenderIcon } from "@/components/ReIcon/src/hooks";
import { PureTableBar } from "@/components/RePureTableBar";
import Password from "@iconify-icons/ri/lock-password-line";
import Search from "@iconify-icons/ep/search";
import Refresh from "@iconify-icons/ep/refresh";
import AddFill from "@iconify-icons/ri/add-circle-line";
import { useUser } from "./utils/hook";
import AdminOutlined from "@iconify-icons/eos-icons/admin-outlined";
import { ref } from "vue";

defineOptions({
  name: "Users"
});

const formRef = ref();
const {
  form,
  loading,
  columns,
  dataList,
  pagination,
  onSearch,
  resetForm,
  handleSizeChange,
  handleCurrentChange,
  handleSelectionChange,
  resetPassword,
  assignRoles,
  openDialog
} = useUser();
</script>

<template>
  <div class="main">
    <el-form
      ref="formRef"
      :inline="true"
      :model="form"
      class="search-form bg-bg_color w-[99/100] pl-8 pt-[12px]"
    >
      <el-form-item label="用户名：" prop="userName">
        <el-input
          v-model="form.userName"
          placeholder="请输入用户名"
          clearable
          class="!w-[200px]"
        />
      </el-form-item>
      <el-form-item label="昵称：" prop="nickName">
        <el-input
          v-model="form.userName"
          placeholder="请输入昵称"
          clearable
          class="!w-[200px]"
        />
      </el-form-item>
      <el-form-item label="性别：" prop="sex">
        <el-select v-model="form.sex" clearable>
          <el-option label="男" value="1" />
          <el-option label="女" value="0" />
        </el-select>
      </el-form-item>
      <el-form-item label="状态：" prop="isEnabled">
        <el-select v-model="form.isEnabled" clearable>
          <el-option label="启用" value="1" />
          <el-option label="禁用" value="0" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button
          type="primary"
          :icon="useRenderIcon(Search)"
          :loading="loading"
          @click="onSearch"
        >
          搜索
        </el-button>
        <el-button :icon="useRenderIcon(Refresh)" @click="resetForm(formRef)">
          重置
        </el-button>
      </el-form-item>
    </el-form>
    <PureTableBar title="用户列表" :columns="columns" @refresh="onSearch">
      <template #buttons>
        <el-button
          type="primary"
          :icon="useRenderIcon(AddFill)"
          @click="openDialog()"
          v-auth="'System:User:Add'"
        >
          新增用户
        </el-button>
      </template>
      <template v-slot="{ size, dynamicColumns }">
        <pure-table
          :data="dataList"
          :columns="dynamicColumns"
          :size="size"
          :pagination="pagination"
          :paginationSmall="size === 'small' ? true : false"
          :header-cell-style="{
            background: 'var(--el-fill-color-light)',
            color: 'var(--el-text-color-primary)'
          }"
          @selection-change="handleSelectionChange"
          @page-size-change="handleSizeChange"
          @page-current-change="handleCurrentChange"
        >
          <template #operation="{ row }">
            <el-button
              class="reset-margin"
              link
              type="primary"
              :size="size"
              :icon="useRenderIcon(AdminOutlined)"
              @click="assignRoles(row)"
              v-auth="'System:User:SetUserRole'"
            >
              配置角色
            </el-button>
            <el-button
              class="reset-margin"
              link
              type="primary"
              :size="size"
              :icon="useRenderIcon(Password)"
              @click="resetPassword(row)"
              v-auth="'System:User:UpdatePassword'"
            >
              重置密码
            </el-button>
          </template>
        </pure-table>
      </template>
    </PureTableBar>
  </div>
</template>
