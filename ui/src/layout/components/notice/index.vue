<script setup lang="ts">
import { ref, onMounted } from "vue";
//import { noticesData } from "./data";
import { getHeaderMsg } from "@/api/workspace/notice";
import NoticeList from "./noticeList.vue";
import Bell from "@iconify-icons/ep/bell";
import { useSignalR } from "./utils/hook";

const noticesNum = ref(0);
const notices = ref([]);
const activeKey = ref("1");
const { initialization } = useSignalR();

function initNotices() {
  getHeaderMsg().then(res => {
    notices.value = res.result.map(x => {
      noticesNum.value = x.children.length;
      return {
        key: x.id,
        name: x.title,
        list: x.children.map(s => {
          s.datetime = s.time;
          s.description = s.content;
          return s;
        })
      };
    });
    setTimeout(() => {
      initNotices();
    }, 60000);
  });
}

onMounted(() => {
  initNotices();
  initialization();
});
</script>

<template>
  <el-dropdown trigger="click" placement="bottom-end">
    <span class="dropdown-badge navbar-bg-hover select-none">
      <el-badge :value="noticesNum" :max="99">
        <span class="header-notice-icon">
          <IconifyIconOffline :icon="Bell" />
        </span>
      </el-badge>
    </span>
    <template #dropdown>
      <el-dropdown-menu>
        <el-tabs
          :stretch="true"
          v-model="activeKey"
          class="dropdown-tabs"
          :style="{ width: notices.length === 0 ? '200px' : '330px' }"
        >
          <el-empty
            v-if="notices.length === 0"
            description="暂无消息"
            :image-size="60"
          />
          <span v-else>
            <template v-for="item in notices" :key="item.key">
              <el-tab-pane
                :label="`${item.name}(${item.list.length})`"
                :name="`${item.key}`"
              >
                <el-scrollbar max-height="330px">
                  <div class="noticeList-container">
                    <NoticeList :list="item.list" />
                  </div>
                </el-scrollbar>
              </el-tab-pane>
            </template>
          </span>
        </el-tabs>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<style lang="scss" scoped>
.dropdown-badge {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 48px;
  margin-right: 10px;
  cursor: pointer;

  .header-notice-icon {
    font-size: 18px;
  }
}

.dropdown-tabs {
  .noticeList-container {
    padding: 15px 24px 0;
  }

  :deep(.el-tabs__header) {
    margin: 0;
  }

  :deep(.el-tabs__nav-wrap)::after {
    height: 1px;
  }

  :deep(.el-tabs__nav-wrap) {
    padding: 0 36px;
  }
}
</style>
